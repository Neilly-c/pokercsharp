using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
using mainsource.system.parser;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class CFRPlusFLPostflop : CFRBase {

        const char DEAL_BOARD = 'z';
        FullCardArr fca = new FullCardArr();
        string[] hands;
        const byte CAP = 2;
        const byte SB = 1, BB = 2;

        public CFRPlusFLPostflop() {
            cards = fca.card_arr;
            hands = fca.hand_abbreviated_arr;
        }

        public override NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {       //stackとtoCallは使わない
            NodeGroup nodeGroup = new NodeGroup(history);
            list.Add(nodeGroup);
            char[] action_set = LegalActionSetPostflop(history);
            foreach (char c in action_set) {
                double pot_next = pot;
                switch (c) {
                    case FOLD:
                    case CHECK:
                        pot_next = pot;
                        break;
                    case CALL:
                        pot_next = history.IndexOf('z') == -1 ? pot + SB : pot + BB;
                        break;
                    case RAISE_A:
                        pot_next = history.IndexOf('z') == -1 ? pot + SB * 2 : pot + BB * 2;
                        break;
                }
                nodeGroup.children.Add(CreateNode(ref list, history + c, 0, pot_next, 0));
            }
            return nodeGroup;
        }

        public override Dictionary<string, Node> Run(int iter, string card_str, int pot, int stack) {
            Card[] card_arr = new Card[5];
            StringHandParser.Parse(card_str).CopyTo(card_arr, 0);
            TrainPostflop(iter, card_arr, pot);
            return nodeMap;
        }

        public void TrainPostflop(int iterations, Card[] board, double pot) {

            double util = 0;
            Card[][] dealt_cards = new Card[2][] { new Card[2], new Card[2] };

            for (int i = 0; i < iterations; i++) {
                DealHoleCard(ref dealt_cards, board);
                util += CFRPostflop(dealt_cards, board, "", 1, 1, 1, pot, 0, 0);
            }
            utilPerIterations = util / iterations;
            Debug.WriteLine("Average game value: " + utilPerIterations);
        }

        private void DealHoleCard(ref Card[][] hand, params Card[][] cardsUsed) {
            List<Card> cards_remain = cards.ToList<Card>();
            foreach (Card[] arr in cardsUsed) {
                foreach (Card used in arr) {
                    cards_remain.Remove(used);
                }
            }

            Card[] temp = cards_remain.ToArray();
            int[] rand = GetUniqRandomNumbers(0, temp.Length - 1, 4);
            hand[0][0] = temp[rand[0]];
            hand[0][1] = temp[rand[1]];
            hand[1][0] = temp[rand[2]];
            hand[1][1] = temp[rand[3]];
            if (hand[0][0].CompareTo(hand[0][1]) < 0) {
                Swap(ref hand[0][0], ref hand[0][1]);
            }
            if (hand[1][0].CompareTo(hand[1][1]) < 0) {
                Swap(ref hand[1][0], ref hand[1][1]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cards">Cardクラスで定義される互いのホールカードの配列</param>
        /// <param name="board">Cardクラスのボード配列 長さ5、未dealはnull</param>
        /// <param name="history">アクション履歴、ターンとリバーのディールはz</param>
        /// <param name="p0">p0確率</param>
        /// <param name="p1">p1確率</param>
        /// <param name="p_">ディール確率</param>
        /// <param name="pot">ポット</param>
        /// <param name="stack">使わない</param>
        /// <param name="toCall">使わない</param>
        /// <returns>そのアクションでの利得を返す</returns>
        private double CFRPostflop(Card[][] cards, Card[] board, string history, double p0, double p1, double p_, double pot, double stack, double toCall) {
            string street_hist = history;
            if (street_hist.Contains('z')) {
                if (street_hist[street_hist.Length - 1].Equals('z')) {
                    street_hist = "";
                } else {
                    street_hist = street_hist.Substring(street_hist.LastIndexOf('z') + 1);
                }
            }

            if (IsActionClosedPostflop(street_hist)) {      //ディーラー手番

                if (street_hist.Length != 0) {
                    switch (street_hist[street_hist.Length - 1]) {
                        case FOLD:
                            if (IsFlop(history)) {        //flop
                                return (pot - SB) / 2;
                            } else {        //turn, river
                                return (pot - BB) / 2;
                            }
                        case CHECK:
                        case CALL:
                            if (history.Split('z').Length == 3) {
                                HoldemHandEvaluator hhe = new HoldemHandEvaluator();
                                FinalHand
                                    fh_pl = hhe.Evaluate(board, cards[0]),
                                    fh_op = hhe.Evaluate(board, cards[1]);
                                if (fh_pl.Equals(fh_op)) {
                                    return 0;
                                } else {
                                    return fh_pl.CompareTo(fh_op) > 0 ? pot / 2 : -pot / 2;       //SD
                                }
                            }
                            break;
                    }
                }

                string board_str = "";
                for (int i = 0; i < board.Length; ++i) {
                    if (Array.IndexOf(board, null) != -1 && i >= Array.IndexOf(board, null)) {
                        break;
                    }
                    board_str += board[i].ToAbbreviateString();
                }
                string infoSet = board_str + "," + history;
                if (!nodeMap.ContainsKey(infoSet)) {
                    Node newNode = new Node(new char[] { DEAL_BOARD }, false, -1);     //たくさんある
                    newNode.infoSet = infoSet;
                    nodeMap.Add(infoSet, newNode);
                }
                Node node = nodeMap[infoSet];

                Card next_card = DealBoard(board, cards[0], cards[1]);
                Card[] board_next = new Card[5];
                Array.Copy(board, board_next, 5);
                if (Array.IndexOf(board, null) == 3) {          //turn
                    board_next[3] = next_card;
                } else if (Array.IndexOf(board, null) == 4) {   //river
                    board_next[4] = next_card;
                }

                string nextHistory = history + 'z';
                double nodeUtil = CFRPostflop(cards, board_next, nextHistory, p0, p1,
                    p_ / (IsRiver(history) ? 44.0d : 45.0d),
                    pot, 0, 0);
                return nodeUtil;

            } else {
                int plays = street_hist.Length;
                int player = plays % 2;
                int opponent = 1 - player;

                string hands_str = cards[player][0].ToAbbreviateString() + cards[player][1].ToAbbreviateString();
                string board_str = "";
                for (int i = 0; i < board.Length; ++i) {
                    if (i >= Array.IndexOf(board, null)) {
                        break;
                    }
                    board_str += board[i].ToAbbreviateString();
                }

                char[] action_set_applied = LegalActionSetPostflop(street_hist);
                string infoSet = hands_str + "," + board_str + "," + history;
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
                    double pot_next = 0;
                    switch (action_set_applied[a]) {
                        case FOLD:
                        case CHECK:
                            pot_next = pot;
                            break;
                        case CALL:
                            pot_next = pot + toCall;
                            break;
                        case RAISE_A:
                            pot_next = (pot + toCall) * (1 + RAISE_A_RATIO);
                            break;
                    }
                    util[a] = player == 0
                        ? -CFRPostflop(cards, board, nextHistory, p0 * strategy[a], p1, p_, pot_next, 0, 0)
                        : -CFRPostflop(cards, board, nextHistory, p0, p1 * strategy[a], p_, pot_next, 0, 0);
                    nodeUtil += strategy[a] * util[a];
                }

                for (int a = 0; a < action_set_applied.Length; a++) {
                    double regret = util[a] - nodeUtil;
                    node.regretSum[a] += (player == 0 ? p1 : p0) * regret;
                }

                return nodeUtil;
            }
        }

        private readonly char[][] ActionSetPostflop = new char[][] {
            new char[]{ CHECK, RAISE_A },     //null
            new char[]{ },                    //f
            new char[]{ CHECK, RAISE_A },     //x
            new char[]{ },                    //c
            new char[]{ FOLD, CALL, RAISE_A },//r
            new char[]{ FOLD, CALL }          //cap
        };

        /// <summary>
        /// 次に行えるアクション一覧を返す
        /// </summary>
        /// <param name="history">そのストリートのここまでの履歴</param>
        /// <returns>charのセット</returns>
        private char[] LegalActionSetPostflop(string history) {   //現時点でのstackとpot
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
                        if (history.Length >= CAP) {       //rrrr
                            return history[history.Length - CAP].Equals(RAISE_A) ? ActionSetPostflop[5] : ActionSetPostflop[4];
                        }
                        return ActionSetPostflop[4];
                }
            }
            return new char[] { };
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

        private bool IsFlop(string history) {
            return history.IndexOf('z') == -1;
        }

        private bool IsRiver(string history) {
            return history.Split('z').Length >= 3;
        }

        /// <summary>
        /// ターンかリバーを配る　どちらも配れない場合は例外を投げる
        /// </summary>
        /// <param name="board">ボード、長さ5</param>
        /// <param name="cardsUsed">boardを除く使用済みカードの配列を列挙</param>
        private Card DealBoard(Card[] board, params Card[][] cardsUsed) {
            List<Card> cards_remain = cards.ToList<Card>();
            foreach (Card b in board) {
                cards_remain.Remove(b);
            }
            foreach (Card[] arr in cardsUsed) {
                foreach (Card used in arr) {
                    cards_remain.Remove(used);
                }
            }

            Random random = new Random();
            Card[] temp = cards_remain.ToArray();
            return temp[random.Next(temp.Length)];
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
