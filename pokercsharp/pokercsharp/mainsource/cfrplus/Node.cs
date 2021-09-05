using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class Node {
        public char[] Action_set { get; }
        readonly int NUM_ACTIONS;
        public string infoSet { get; set; }
        public double[] regretSum, strategy, strategySum;

        public Node(char[] action_set) {
            this.Action_set = action_set;
            NUM_ACTIONS = action_set.Length;
            regretSum = new double[NUM_ACTIONS];
            strategy = new double[NUM_ACTIONS];
            strategySum = new double[NUM_ACTIONS];
    }


        public double[] GetStrategy(double realizationWeight) {
            double normalizingSum = 0;
            for (int a = 0; a < NUM_ACTIONS; a++) {
                strategy[a] = regretSum[a] > 0 ? regretSum[a] : 0;
                normalizingSum += strategy[a];
            }
            for (int a = 0; a < NUM_ACTIONS; a++) {
                if (normalizingSum > 0) {
                    strategy[a] /= normalizingSum;
                } else {
                    strategy[a] = 1.0 / NUM_ACTIONS;
                }
                strategySum[a] += realizationWeight * strategy[a];
            }
            return strategy;
        }


        public double[] GetAverageStrategy() {
            double[] avgStrategy = new double[NUM_ACTIONS];
            double normalizingSum = 0;
            for (int a = 0; a < NUM_ACTIONS; a++) {
                normalizingSum += strategySum[a];
            }
            for (int a = 0; a < NUM_ACTIONS; a++) {
                if (normalizingSum > 0) {
                    avgStrategy[a] = strategySum[a] / normalizingSum;
                } else {
                    avgStrategy[a] = 1.0 / NUM_ACTIONS;
                }
            }
            return avgStrategy;
        }

        public override string ToString() {
            string result = "";
            double[] avgStrategy = GetAverageStrategy();
            for(int i = 0; i < NUM_ACTIONS; ++i) {
                result += Action_set[i] + ": " + avgStrategy[i] + " ";
            }
            return result;
        }

    }
}
