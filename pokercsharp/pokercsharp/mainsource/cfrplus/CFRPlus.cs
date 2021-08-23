using System.Diagnostics;

namespace mainsource.cfrplus {

    public class CFRPlus {/*
        public const int FOLD = 0, BET = 1, NUM_ACTIONS = 2;
        public TreeMap<String, Node> nodeMap = new TreeMap<String, Node>();

        class Node {
            string infoSet;
            double[] regretSum = new double[NUM_ACTIONS],
                     strategy = new double[NUM_ACTIONS],
                     strategySum = new double[NUM_ACTIONS];


            private double[] GetStrategy(double realizationWeight) {
                double normalizingSum = 0;
                for (int a = 0; a < NUM_ACTIONS; a++) {
                    strategy[a] = regretSum[a] > 0 ? regretSum[a] : 0;
                    normalizingSum += strategy[a];
                }
                for (int a = 0; a < NUM_ACTIONS; a++) {
                    if (normalizingSum > 0)
                        strategy[a] /= normalizingSum;
                    else
                        strategy[a] = 1.0 / NUM_ACTIONS;
                    strategySum[a] += realizationWeight * strategy[a];
                }
                return strategy;
            }


            public double[] GetAverageStrategy() {
                double[] avgStrategy = new double[NUM_ACTIONS];
                double normalizingSum = 0;
                for (int a = 0; a < NUM_ACTIONS; a++)
                    normalizingSum += strategySum[a];
                for (int a = 0; a < NUM_ACTIONS; a++)
                    if (normalizingSum > 0)
                        avgStrategy[a] = strategySum[a] / normalizingSum;
                    else
                        avgStrategy[a] = 1.0 / NUM_ACTIONS;
                return avgStrategy;
            }


            public string ToString() {
                return string.format("%4s: %s", infoSet, Arrays.toString(GetAverageStrategy()));
            }

        }


        public void train(int iterations) {
            int[] cards = { 1, 2, 3, 4, 5 };
            double util = 0;
            for (int i = 0; i < iterations; i++) {
                for (int c1 = cards.Length - 1; c1 > 0; c1--) {     //シャッフルしてるだけ
                    int c2 = random.nextInt(c1 + 1);
                    int tmp = cards[c1];
                    cards[c1] = cards[c2];
                    cards[c2] = tmp;
                }

                util += cfr(cards, "", 1, 1);   //1回回しつつ戦略を更新する
            }
            Debug.WriteLine("Average game value: " + util / iterations);
            foreach (Node n in nodeMap.values()) {
                System.out.println((n.getAverageStrategy())[1]);
            }
        }


        private double cfr(int[] cards, string history, double p0, double p1) {
            int plays = history.Length;
            int player = plays % 2;
            int opponent = 1 - player;
            if (plays > 1) {
                bool terminalPass = history.charAt(plays - 1) == 'p';
                bool doubleBet = history.substring(plays - 2, plays).equals("bb");
                bool isPlayerCardHigher = cards[player] > cards[opponent];
                if (terminalPass)
                    if (history.equals("pp"))
                        return isPlayerCardHigher ? 1 : -1;     //ショウダウン勝率 実際は互いのハンドに合わせた勝率(-1 ~ 1)をここに放り込めばいい
                    else
                        return 1;
                else if (doubleBet)
                    return isPlayerCardHigher ? 2 : -2;     //ショウダウン勝率 ベット額とショウダウン勝率が絡む部分
            }

            string infoSet = cards[player] + history;
            Node node = nodeMap.get(infoSet);
            if (node == null) {
                node = new Node();
                node.infoSet = infoSet;
                nodeMap.put(infoSet, node);
            }

            double[] strategy = node.GetStrategy(player == 0 ? p0 : p1);
            double[] util = new double[NUM_ACTIONS];
            double nodeUtil = 0;
            for (int a = 0; a < NUM_ACTIONS; a++) {
                String nextHistory = history + (a == 0 ? "p" : "b");
                util[a] = player == 0
                    ? -cfr(cards, nextHistory, p0 * strategy[a], p1)
                    : -cfr(cards, nextHistory, p0, p1 * strategy[a]);
                nodeUtil += strategy[a] * util[a];
            }

            for (int a = 0; a < NUM_ACTIONS; a++) {
                double regret = util[a] - nodeUtil;
                node.regretSum[a] += (player == 0 ? p1 : p0) * regret;
            }

            return nodeUtil;
        }


        public static void main(String[] args) {
            int iterations = 10;
            new Main().train(iterations);
        }*/

    }
}
