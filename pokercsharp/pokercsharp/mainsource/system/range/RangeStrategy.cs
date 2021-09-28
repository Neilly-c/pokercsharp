using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.range {
    class RangeStrategy {
        private Dictionary<string, double>[][] strategy;


        public RangeStrategy() {
            strategy = new Dictionary<string, double>[Constants.CARDVALUE_LEN][];
            for(int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                strategy[i] = new Dictionary<string, double>[Constants.CARDVALUE_LEN];
                for(int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    strategy[i][j] = new Dictionary<string, double>();
                }
            }
        }

        public void SetStrategy(int row, int col, double[] val, string[] actions) {
            Dictionary<string, double> targetDict = strategy[row][col];
            targetDict.Clear();
            for(int i = 0; i < val.Length; ++i) {
                if(i >= actions.Length) {
                    break;
                }
                targetDict.Add(actions[i], val[i]);
            }
        }

        public Dictionary<string, double> GetStrategyDict(int row, int col) {
            Dictionary<string, double> targetDict = strategy[row][col];
            return targetDict;
        }
    }
}
