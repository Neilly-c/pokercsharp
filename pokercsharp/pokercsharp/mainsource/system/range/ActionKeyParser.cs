using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.range {
    public class ActionKeyParser {
        private int[][] actions;
        /*
         * fold = -1
         * call = 1
         * raise = 2
         */ 
        private readonly int PLAYERS, ACTION_CAP;

        public ActionKeyParser(int players, int actionCap) {
            this.PLAYERS = players;
            this.ACTION_CAP = actionCap;
            actions = new int[ACTION_CAP][];
            for(int i = 0; i < ACTION_CAP; ++i) {
                actions[i] = new int[PLAYERS];
            }
        }

        public void reset() {
            for(int i = 0; i < ACTION_CAP; ++i) {
                for(int j = 0; j < PLAYERS; ++j) {
                    actions[i][j] = 0;
                }
            }
        }

        /// <summary>
        /// Call when some player raise
        /// </summary>
        /// <param name="player">where 0 is UTG, 3 is BTN, 4 is SB and 5 is BB</param>
        /// <param name="betNumber">open raise(2-bet) is 1, 3bet is 2.</param>
        public void raise(int player, int betNumber) {
            bool flag = false;
            for(int i = 0; i < actions[betNumber].Length; ++i) {
                if(actions[betNumber][i] == 2) {
                    flag = true;
                }
            }
            if (flag) {

            } else {
                switch (betNumber) {
                    case 1:
                        actions[betNumber][player] = 2;
                        for(int i = 0; i < player; ++i) {
                            actions[0][i] = -1;
                        }
                        break;
                }
            }
        }
    }
}
