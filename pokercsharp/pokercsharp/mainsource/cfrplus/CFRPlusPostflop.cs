using mainsource.system.card;
using mainsource.system.parser;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class CFRPlusPostflop : CFRBase {

        const char DEAL_BOARD = 'z';
        FullCardArr fca = new FullCardArr();
        string[] hands;

        public CFRPlusPostflop() {
            cards = fca.card_arr;
            hands = fca.hand_abbreviated_arr;
        }

        public override NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {
            NodeGroup nodeGroup = new NodeGroup(history);
            list.Add(nodeGroup);
            char[] action_set = LegalActionSetPostflop(history, stack, pot, toCall);
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

        public override Dictionary<string, Node> Run(int iter, string card_str, int pot, int stack) {
            this.stack = stack;
            Card[] card_arr = new Card[5];
            StringHandParser.Parse(card_str).CopyTo(card_arr, 0);
            TrainPostflop(iter, card_arr, pot, stack);
            return nodeMap;
        }

        public void TrainPostflop(int iterations, Card[] board, double pot, double stack) {

            double util = 0;
            Random random = new Random();
            Card[][] dealt_cards = new Card[2][] { new Card[2], new Card[2] };

            for (int i = 0; i < iterations; i++) {
                int[] rand = GetUniqRandomNumbers(0, cards.Length - 1, 4);
                dealt_cards[0][0] = cards[rand[0]];
                dealt_cards[0][1] = cards[rand[1]];
                dealt_cards[1][0] = cards[rand[2]];
                dealt_cards[1][1] = cards[rand[3]];
                if (dealt_cards[0][0].CompareTo(dealt_cards[0][1]) < 0) {
                    Swap(ref dealt_cards[0][0], ref dealt_cards[0][1]);
                }
                if (dealt_cards[1][0].CompareTo(dealt_cards[1][1]) < 0) {
                    Swap(ref dealt_cards[1][0], ref dealt_cards[1][1]);
                }
                util += CFRPostflop(dealt_cards, board, "", 1, 1, 1, pot, stack, 0);
            }
            utilPerIterations = util / iterations;
            Debug.WriteLine("Average game value: " + utilPerIterations);
        }

        private double CFRPostflop(Card[][] cards, Card[] board, string history, double p0, double p1, double p_, double pot, double stack, double toCall) {
            string street_hist = history;
            if (street_hist.Contains('z')) {
                if (street_hist[street_hist.Length - 1].Equals('z')) {
                    street_hist = "";
                } else {
                    street_hist = street_hist.Substring(street_hist.LastIndexOf('z') + 1);
                }
            }
            int plays = street_hist.Length;
            int player = plays % 2;
            int opponent = 1 - player;

            if (plays > 0) {
                int a = Constants.handKey[cards[player][0].ToAbbreviateString()[0]],
                    b = Constants.handKey[cards[player][1].ToAbbreviateString()[0]],
                    c = Constants.handKey[cards[opponent][0].ToAbbreviateString()[0]],
                    d = Constants.handKey[cards[opponent][1].ToAbbreviateString()[0]];
                if (a != b) {
                    if (!cards[player][0].GetSuit().Equals(cards[player][1].GetSuit())) {
                        Swap(ref a, ref b);
                    }
                }
                if (c != d) {
                    if (!cards[opponent][0].GetSuit().Equals(cards[opponent][1].GetSuit())) {
                        Swap(ref c, ref d);
                    }
                }
                if (street_hist.Length != 0) {
                    switch (street_hist[street_hist.Length - 1]) {
                        case 'f':
                            return (pot - toCall) + stack * 2 - stack;
                        case 'c':
                            return 0;
                    }
                }
            }

            string hands_str = cards[player][0].ToAbbreviateString() + cards[player][1].ToAbbreviateString();
            string board_str = "";
            for (int i = 0; i < board.Length; ++i) {
                if(i >= Array.IndexOf(board, null)) {
                    break;
                }
                board_str += board[i].ToAbbreviateString();
            }

            if (IsActionClosedPostflop(street_hist)) {

                string infoSet = hands_str + board_str + history;
                if (!nodeMap.ContainsKey(infoSet)) {
                    Node newNode = new Node(new char[] { DEAL_BOARD }, false, -1);     //たくさんある
                    newNode.infoSet = infoSet;
                    nodeMap.Add(infoSet, newNode);
                }
                Node node = nodeMap[infoSet];

                Card next_board = DealNewCard(board, cards[0], cards[1]);
                if (Array.IndexOf(board, null) == 3) {        //turn
                    board[3] = next_board;
                    string nextHistory = history + next_board.ToAbbreviateString() + 'z';
                    double nodeUtil = CFRPostflop(cards, board, nextHistory, p0, p1, p_ / 45.0d, pot, stack, 0);
                    return nodeUtil;
                } else if (Array.IndexOf(board, null) == 4) {                            //river
                    board[4] = next_board;
                    string nextHistory = history + next_board.ToAbbreviateString() + 'z';
                    double nodeUtil = CFRPostflop(cards, board, nextHistory, p0, p1, p_ / 44.0d, pot, stack, 0);
                    return nodeUtil;
                }

                return 0;   // ?

            } else {

                char[] action_set_applied = LegalActionSetPostflop(street_hist, stack, pot, toCall);
                string infoSet = hands_str + board_str + history;
                if (!nodeMap.ContainsKey(infoSet)) {
                    Node newNode = new Node(action_set_applied, false, player);
                    newNode.infoSet = infoSet;
                    nodeMap.Add(infoSet, newNode);
                }
                Node node = nodeMap[infoSet];

                double[] strategy = node.GetStrategy(player == 0 ? p0 : p1);
                double[] util = new double[action_set_applied.Length];
                double nodeUtil = 0;
                for (int a = 0; a < action_set_applied.Length; a++) {
                    string nextHistory = history + action_set_applied[a];
                    double stack_next = 0, pot_next = 0, toCall_next = 0;
                    switch (action_set_applied[a]) {
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
                    util[a] = player == 0
                        ? -CFRPostflop(cards, board, nextHistory, p0 * strategy[a], p1, 1, pot_next, stack_next, toCall_next)
                        : -CFRPostflop(cards, board, nextHistory, p0, p1 * strategy[a], 1, pot_next, stack_next, toCall_next);
                    nodeUtil += strategy[a] * util[a] * p_;
                }

                for (int a = 0; a < action_set_applied.Length; a++) {
                    double regret = util[a] - nodeUtil;
                    node.regretSum[a] += (player == 0 ? p1 : p0) * regret;
                }

                return nodeUtil;

            }
        }

        private readonly char[][] ActionSetPostflop = new char[][] {
            new char[]{ CHECK, RAISE_A, PUSH },     //null
            new char[]{ },                          //f
            new char[]{ CHECK, RAISE_A, PUSH },     //x
            new char[]{ },                          //c
            new char[]{ FOLD, CALL, RAISE_A, PUSH },//r
            new char[]{ FOLD, CALL },               //p
            new char[]{ FOLD, CALL, PUSH }          //r force to push
        };

        private char[] LegalActionSetPostflop(string history, double stack, double pot, double toCall) {   //現時点でのstackとpot
            if (history.Length == 0) {
                return ActionSetPostflop[0];
            } else {
                switch (history[history.Length - 1]) {
                    case FOLD:
                        return ActionSetPostflop[1];
                    case CHECK:
                        return history.Length == 1 ? ActionSetPostflop[2] : ActionSetPostflop[1];     //xxの場合はアクションを閉じる、でなければオプション
                    case CALL:
                        return ActionSetPostflop[3];
                    case RAISE_A:
                        return IsRaiseLegal(pot, stack, toCall, RAISE_A_RATIO) ? ActionSetPostflop[4] : ActionSetPostflop[6];
                    case PUSH:
                        return ActionSetPostflop[5];
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
            Card[] temp = cards_remain.ToArray();
            Card c = temp[random.Next(temp.Length)];
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
