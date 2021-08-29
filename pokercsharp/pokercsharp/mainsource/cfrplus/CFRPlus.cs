using pokercsharp.mainsource.cfrplus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mainsource.cfrplus {

    public class CFRPlus {

        public const int FOLD = 0, PUSH = 1, NUM_ACTIONS = 2;
        public Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();

        public void Train(int iterations) {
            int[] cards = { 1, 2, 3 };
            double util = 0;
            for (int i = 0; i < iterations; i++) {

                Random random = new Random();
                cards = cards.OrderBy(x => random.Next()).ToArray();    //シャッフル

                util += CFR(cards, "", 1, 1, 1);   //1回回しつつ戦略を更新する
            }
            Debug.WriteLine("Average game value: " + util / iterations);
            foreach (string key in nodeMap.Keys) {
                Debug.WriteLine( key + ": " + (nodeMap[key].GetAverageStrategy())[1]);
            }
        }

        /*
         * history = F or P (fold or push)
        */
        private double CFR(int[] cards, string history, double p0, double p1, double p_) {
            int plays = history.Length;
            int player = plays % 2;
            int opponent = 1 - player;

            if(plays > 0) {
                bool isPlayerCardHigher = cards[player] > cards[opponent];
                switch (history) {
                    case "pp":
                        return isPlayerCardHigher ? 4 : -4;
                    case "pf":
                        return 2;
                    case "f":
                        return 1;
                }
            }

            string infoSet = cards[player] + history;
            if (!nodeMap.ContainsKey(infoSet)) {
                Node newNode = new Node();
                newNode.infoSet = infoSet;
                nodeMap.Add(infoSet, newNode);
            }
            Node node = nodeMap[infoSet];

            double[] strategy = node.GetStrategy(player == 0 ? p0 : p1);
            double[] util = new double[NUM_ACTIONS];
            double nodeUtil = 0;
            for (int a = 0; a < NUM_ACTIONS; a++) {
                string nextHistory = history + (a == 0 ? "f" : "p");
                util[a] = player == 0
                    ? -CFR(cards, nextHistory, p0 * strategy[a], p1, 1)
                    : -CFR(cards, nextHistory, p0, p1 * strategy[a], 1);
                nodeUtil += strategy[a] * util[a];
            }

            for (int a = 0; a < NUM_ACTIONS; a++) {
                double regret = util[a] - nodeUtil;
                node.regretSum[a] += (player == 0 ? p1 : p0) * regret;
            }

            return nodeUtil;
        }


        public void Run(int iter) {
            Train(iter);
        }

    }
}
