using pokercsharp.mainsource;
using pokercsharp.mainsource.cfrplus;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mainsource.cfrplus {

    public class CFRPlus {

        const int FOLD = 0, PUSH = 1, NUM_ACTIONS = 2;
        Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
        double stack = 2, bb = 1, sb = 0.5d, ante = 0;
        FullCardArr fca = new FullCardArr();
        AggregatedWinRateGrid agwr = new AggregatedWinRateGrid();
        string[] cards;
        int[][][][] digest_grid;
        int[][][][] digest_count;
        public double utilPerIterations { get; set; }

        public CFRPlus() {
            cards = fca.hand_abbreviated_arr;
            agwr.Init();
            digest_grid = agwr.Get_digest_grid();
            digest_count = agwr.Get_digest_count();
        }

        public void Train(int iterations) {

            double util = 0;
            Random random = new Random();
            string[] dealt_cards = new string[2];

            for (int i = 0; i < iterations; i++) {
                dealt_cards[0] = cards[random.Next(cards.Length)];
                dealt_cards[1] = cards[random.Next(cards.Length)];
                util += CFR(dealt_cards, "", 1, 1, 1);   //1回回しつつ戦略を更新する
            }
            utilPerIterations = util / iterations;
            Debug.WriteLine("Average game value: " + utilPerIterations);
            foreach (string key in nodeMap.Keys) {
                Debug.WriteLine( key + ": " + nodeMap[key].GetAverageStrategy()[1]);
            }
        }

        /*
         * history = F or P (fold or push)
        */
        private double CFR(string[] cards, string history, double p0, double p1, double p_) {
            int plays = history.Length;
            int player = plays % 2;
            int opponent = 1 - player;

            if(plays > 0) {
                int a = Constants.handKey[cards[player][0]],
                    b = Constants.handKey[cards[player][1]],
                    c = Constants.handKey[cards[opponent][0]],
                    d = Constants.handKey[cards[opponent][1]];
                if(a != b) {
                    if (cards[player][2].Equals('o')) {
                        int temp = a; a = b; b = temp;
                    }
                }
                if (c != d) {
                    if (cards[opponent][2].Equals('o')) {
                        int temp = c; c = d; d = temp;
                    }
                }
                //bool isPlayerCardHigher = cards[player] > cards[opponent];
                switch (history) {
                    /*  kuhn poker
                    case "ff":
                        return isPlayerCardHigher ? 1 : -1;
                    case "fpp":
                    case "pp":
                        return isPlayerCardHigher ? 2 : -2;
                    case "fpf":
                    case "pf":
                        return 1;
                    */
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


        public Dictionary<string, Node> Run(int iter, int stack) {
            this.stack = stack;
            Train(iter);
            return nodeMap;
        }

        public void refreshNodeMap() {
            nodeMap.Clear();
        }

    }
}
