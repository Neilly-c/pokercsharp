using mainsource.system.card;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    class CFRAoFPreflop : CFRBase {

        FullCardArr fca = new FullCardArr();
        WinRateGrid13Reader wrgReader = new WinRateGrid13Reader();
        string[] hands;
        int[][][][] digest_grid;
        int[][][][] digest_count;
        char[] action_set = new char[] { FOLD, PUSH };

        public CFRAoFPreflop() {
            cards = fca.card_arr;
            hands = fca.hand_abbreviated_arr;
            wrgReader.Init();
            digest_grid = wrgReader.Get_win_count_13();
            digest_count = wrgReader.Get_match_up_count_13();
        }

        public override NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {
            NodeGroup nodeGroup = new NodeGroup(history);
            list.Add(nodeGroup);
            foreach (char c in action_set) {
                switch (c) {
                    case FOLD:
                        break;
                    case PUSH:
                        if(history.Length < 1) 
                            nodeGroup.children.Add(CreateNode(ref list, history + c, stack, pot + stack, stack));
                        break;
                }
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
            int plays = history.Length; //ここまでアクションがあった回数
            int player = plays % 2;     //手番のプレイヤー
            int opponent = 1 - player;  //手番ではないプレイヤー

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
                switch (history) {  //全てのアクションが出揃った時はそのノードの利得を返す
                    case "pp":
                        return -stack + ((double)stack * (double)digest_grid[a][b][c][d] / ((double)Constants._48C5 * (double)digest_count[a][b][c][d]));
                    case "pf":
                        return bb;
                    case "f":
                        return sb;
                }
            }

            string infoSet = cards[player] + history;
            if (!nodeMap.ContainsKey(infoSet)) {
                Node newNode = new Node(action_set, true, player);      //手番のプレイヤーと取れるアクションを保持したノードを作る
                newNode.infoSet = infoSet;                              //ノードに履歴を与える
                nodeMap.Add(infoSet, newNode);
            }
            Node node = nodeMap[infoSet];       //手番プレイヤー、可能なアクション、履歴の情報を持っている

            double[] strategy = node.GetStrategy(player == 0 ? p0 : p1);    //ある戦略を取る確率
            double[] util = new double[action_set.Length];                  //ある戦略を取った時の利得の配列
            double nodeUtil = 0;                                            //ノードの利得
            for (int a = 0; a < action_set.Length; a++) {
                string nextHistory = history + action_set[a];
                util[a] = player == 0
                    ? -CFRPreflop(cards, nextHistory, p0 * strategy[a], p1, 1)
                    : -CFRPreflop(cards, nextHistory, p0, p1 * strategy[a], 1);
                nodeUtil += strategy[a] * util[a];                          //戦略ごとの利得×その選択を取る確率を総和してノードの利得とする
            }

            for (int a = 0; a < action_set.Length; a++) {
                double regret = util[a] - nodeUtil;                         //後悔値。ある戦略に対して、その戦略を取った時の利得とノード全体の利得の差で、
                                                                            //これが負だとその戦略はいらないということになる
                node.regretSum[a] += (player == 0 ? p1 : p0) * regret;      //0以上を足さないといけないのでは？わからない
            }

            return nodeUtil;
        }

        static void Swap<T>(ref T m, ref T n) {
            T work = m;
            m = n;
            n = work;
        }

    }
}
