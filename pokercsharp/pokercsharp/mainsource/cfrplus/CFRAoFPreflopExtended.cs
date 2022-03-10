using mainsource.system.card;
using mainsource.system.game;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    class CFRAoFPreflopExtended : CFRBase {

        FullCardArr fca = new FullCardArr();
        string[] hands;
        char[] action_set = new char[] { FOLD, PUSH };
        int players { get; set; }
        NewDeck deck = new NewDeck();

        public CFRAoFPreflopExtended() {
            cards = fca.card_arr;
            hands = fca.hand_abbreviated_arr;
            players = 2;
        }

        public override NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {
            NodeGroup nodeGroup = new NodeGroup(history);
            list.Add(nodeGroup);
            if (history.Length < players) {       //プレイヤー数までしかアクションが発生しない
                foreach (char c in action_set) {
                    nodeGroup.children.Add(CreateNode(ref list, history + c, stack, pot + stack, stack));
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

            double[] util = new double[players];
            Card[][] dealt_cards_def = new Card[players][];
            for (int i = 0; i < players; ++i) {
                dealt_cards_def[i] = new Card[2];
            }
            string[] dealt_cards = new string[players];

            for (int i = 0; i < iterations; i++) {

                deck.ReShuffle();
                for(int j = 0; j < players; ++j) {
                    dealt_cards_def[j][0] = deck.Deal1();
                    dealt_cards_def[j][1] = deck.Deal1();
                    dealt_cards[j] = CardToHandConverter.ConvertCardsToHandStr(dealt_cards_def[j]);
                }

                double[] p = new double[players];
                Array.Fill(p, 1);
                util += CFRPreflop(dealt_cards, "", p);   //1回回しつつ戦略を更新する
            }
            utilPerIterations = util / iterations;
            Debug.WriteLine("Average game value: " + utilPerIterations);
            /*foreach (string key in nodeMap.Keys) {
                Debug.WriteLine( key + ": " + nodeMap[key].GetAverageStrategy()[1]);
            }*/
        }

        private double[] CFRPreflop(string[] cards, string history, double[] p) {
            int plays = history.Length; //ここまでアクションがあった回数
            int currentPlayer = plays;     //手番のプレイヤー
            //int opponent = 1 - player;  //手番ではないプレイヤー

            if (plays == players) {
                //全てのアクションが出揃った時はそのノードの利得を返す
                
            }

            string infoSet = cards[currentPlayer] + history;
            if (!nodeMap.ContainsKey(infoSet)) {
                Node newNode = new Node(action_set, true, currentPlayer);      //手番のプレイヤーと取れるアクションを保持したノードを作る
                newNode.infoSet = infoSet;                              //ノードに履歴を与える
                nodeMap.Add(infoSet, newNode);
            }
            Node node = nodeMap[infoSet];       //手番プレイヤー、可能なアクション、履歴の情報を持っている

            double[] strategy = node.GetStrategy(p[currentPlayer]);    //ある戦略を取る確率
            double[] util = new double[action_set.Length];                  //ある戦略を取った時の利得の配列
            double nodeUtil = 0;                                            //ノードの利得
            for (int a = 0; a < action_set.Length; a++) {
                string nextHistory = history + action_set[a];
                p[currentPlayer] *= strategy[a];
                util[a] = -CFRPreflop(cards, nextHistory, p);
                nodeUtil += strategy[a] * util[a];                          //戦略ごとの利得×その選択を取る確率を総和してノードの利得とする
            }

            for (int a = 0; a < action_set.Length; a++) {
                double regret = util[a] - nodeUtil;                         //後悔値。ある戦略に対して、その戦略を取った時の利得とノード全体の利得の差で、
                                                                            //これが負だとその戦略はいらないということになる
                double pSum = 1;
                for(int i = 0; i < currentPlayer; ++i) {
                    pSum *= p[i];
                }
                node.regretSum[a] += pSum * regret;      //0以上を足さないといけないのでは？わからない
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
