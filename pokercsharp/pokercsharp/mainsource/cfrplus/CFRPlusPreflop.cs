using mainsource.system.card;
using pokercsharp.mainsource;
using pokercsharp.mainsource.cfrplus;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mainsource.cfrplus {

    public class CFRPlusPreflop : CFRBase {

        FullCardArr fca = new FullCardArr();
        AggregatedWinRateGrid agwr = new AggregatedWinRateGrid();
        string[] hands;
        int[][][][] digest_grid;
        int[][][][] digest_count;

        public CFRPlusPreflop() {
            cards = fca.card_arr;
            hands = fca.hand_abbreviated_arr;
            agwr.Init();
            digest_grid = agwr.Get_digest_grid();
            digest_count = agwr.Get_digest_count();
        }

        public override NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {
            NodeGroup nodeGroup = new NodeGroup(history);
            list.Add(nodeGroup);
            char[] action_set = LegalActionSetPreflop(history);
            foreach (char c in action_set) {
                double stack_next = stack, pot_next = pot, toCall_next = toCall;
                switch (c) {
                    case FOLD:
                        stack_next = stack - toCall;
                        pot_next = pot;
                        toCall_next = 0;
                        break;
                    case CHECK:
                        stack_next = stack;
                        pot_next = pot;
                        toCall_next = 0;
                        break;
                    case CALL:
                        stack_next = stack - toCall;
                        pot_next = pot + toCall;
                        toCall_next = 0;
                        break;
                    case RAISE_A:
                        stack_next = stack - toCall;
                        pot_next = (pot + toCall) * (1 + RAISE_A_RATIO);
                        toCall_next = pot_next - pot;
                        break;
                    case PUSH:
                        stack_next = stack - toCall;
                        pot_next = pot + stack;
                        toCall_next = stack_next;
                        break;
                }
                nodeGroup.children.Add(CreateNode(ref list, history + c, stack_next, pot_next, toCall_next));
            }
            return nodeGroup;
        }

        public override Dictionary<string, Node> Run(int iter, int stack) {
            this.stack = stack;
            TrainPreflop(iter);
            return nodeMap;
        }

        public void TrainPreflop(int iterations) {

            double util = 0;
            Random random = new Random();
            string[] dealt_cards = new string[2];

            for (int i = 0; i < iterations; i++) {
                dealt_cards[0] = hands[random.Next(hands.Length)];
                dealt_cards[1] = hands[random.Next(hands.Length)];
                util += CFRPreflop(dealt_cards, "", 1, 1, 1);   //1回回しつつ戦略を更新する
            }
            utilPerIterations = util / iterations;
            Debug.WriteLine("Average game value: " + utilPerIterations);
            /*foreach (string key in nodeMap.Keys) {
                Debug.WriteLine( key + ": " + nodeMap[key].GetAverageStrategy()[1]);
            }*/
        }

        private double CFRPreflop(string[] cards, string history, double p0, double p1, double p_) {
            int plays = history.Length;
            int player = plays % 2;
            int opponent = 1 - player;

            if (plays > 0) {
                int a = Constants.handKey[cards[player][0]],
                    b = Constants.handKey[cards[player][1]],
                    c = Constants.handKey[cards[opponent][0]],
                    d = Constants.handKey[cards[opponent][1]];
                if (a != b) {
                    if (cards[player][2].Equals('o')) {
                        Swap(ref a, ref b);
                    }
                }
                if (c != d) {
                    if (cards[opponent][2].Equals('o')) {
                        Swap(ref c, ref d);
                    }
                }
                switch (history) {
                    case "pp":
                    case "rpp":
                        return -stack + ((double)stack * (double)digest_grid[a][b][c][d] / ((double)Constants._48C5 * (double)digest_count[a][b][c][d]));
                    case "rpf":
                        return 2 * bb;
                    case "pf":
                    case "rf":
                        return bb;
                    case "f":
                        return sb;
                }
            }

            char[] action_set_applied = LegalActionSetPreflop(history);

            string infoSet = cards[player] + history;
            if (!nodeMap.ContainsKey(infoSet)) {
                Node newNode = new Node(action_set_applied, true, player);
                newNode.infoSet = infoSet;
                nodeMap.Add(infoSet, newNode);
            }
            Node node = nodeMap[infoSet];

            double[] strategy = node.GetStrategy(player == 0 ? p0 : p1);
            double[] util = new double[action_set_applied.Length];
            double nodeUtil = 0;
            for (int a = 0; a < action_set_applied.Length; a++) {
                string nextHistory = history + action_set_applied[a];
                util[a] = player == 0
                    ? -CFRPreflop(cards, nextHistory, p0 * strategy[a], p1, 1)
                    : -CFRPreflop(cards, nextHistory, p0, p1 * strategy[a], 1);
                nodeUtil += strategy[a] * util[a];
            }

            for (int a = 0; a < action_set_applied.Length; a++) {
                double regret = util[a] - nodeUtil;
                node.regretSum[a] += (player == 0 ? p1 : p0) * regret;
            }

            return nodeUtil;
        }

        private readonly char[][] ActionSetPreflop = new char[][] {
            new char[]{ FOLD, CALL, RAISE_A, PUSH },    //null
            new char[]{ },                              //f
            new char[]{ },                              //x
            new char[]{ CHECK, RAISE_A, PUSH },         //c
            new char[]{ FOLD, CALL, RAISE_A, PUSH },    //r
            new char[]{ FOLD, CALL }                    //p
        };

        private readonly char[][] ActionSetPreflop_denyCall = new char[][] {
            new char[]{ FOLD, RAISE_A, PUSH },          //null
            new char[]{ },                              //f
            new char[]{ FOLD, PUSH },                   //r
            new char[]{ FOLD, PUSH }                    //p
        };

        private char[] LegalActionSetPreflop(string history) {
            if (history.Length == 0) {
                return ActionSetPreflop_denyCall[0];
            } else {
                switch (history[history.Length - 1]) {
                    case FOLD:
                        return ActionSetPreflop_denyCall[1];
                    case RAISE_A:
                        return ActionSetPreflop_denyCall[2];
                    case PUSH:
                        return history.Length == 1 || !history[history.Length - 2].Equals(PUSH) ? ActionSetPreflop_denyCall[3] : new char[] { };
                }
            }
            return new char[] { };
        }

        private bool IsRaiseLegal(double pot, double stack, double toCall) {
            double toMinRaise = toCall * 2;
            return toMinRaise <= stack;
        }

        private bool IsRaiseLegal(double pot, double stack, double toCall, double potRatio) {
            double RequiredRaise = (pot + toCall) * (1 + potRatio) - pot;
            return RequiredRaise <= stack;
        }

        private bool IsActionClosedPostflop(string history) {
            if (history.Length >= 2) {
                char lastChar = history[history.Length - 1];
                if (lastChar.Equals('f') || lastChar.Equals('c')) {
                    return true;
                } else if (history[history.Length - 2].Equals('x') && lastChar.Equals('x')) {
                    return true;
                }
            } else if (history.Length == 1) {
                return history.Equals("f");
            }
            return false;
        }

        private Card DealNewCard(params Card[][] cardsUsed) {
            List<Card> cards_remain = cards.ToList<Card>();
            foreach (Card[] arr in cardsUsed) {
                foreach (Card used in arr) {
                    cards_remain.Remove(used);
                }
            }

            Random random = new Random();
            Card c = cards_remain.ToArray()[random.Next(cards.Length)];
            return c;
        }

        static void Swap<T>(ref T m, ref T n) {
            T work = m;
            m = n;
            n = work;
        }

        static int[] GetUniqRandomNumbers(int rangeBegin, int rangeEnd, int count) {
            int[] work = new int[rangeEnd - rangeBegin + 1];

            for (int n = rangeBegin, i = 0; n <= rangeEnd; n++, i++)
                work[i] = n;

            var rnd = new Random();
            for (int resultPos = 0; resultPos < count; resultPos++) {
                int nextResultPos = rnd.Next(resultPos, work.Length);
                Swap(ref work[resultPos], ref work[nextResultPos]);
            }

            return work.Take(count).ToArray();
        }

    }
}
