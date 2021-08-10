using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace pokercsharp.mainsource.appendix {
    class WinRateGrid {
        const int COMBINATION = 1326;
        const int FULL_DECK_LEN = 52;

        public void Init() {
            int[][] full_grid = new int[COMBINATION][];
            Card[][] hand_arr = new Card[COMBINATION][];
            for (int i = 0; i < COMBINATION; ++i) {
                full_grid[i] = new int[COMBINATION];
                hand_arr[i] = new Card[2];
            }

            int iter = 0;
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                for (int j = i + 1; j < FULL_DECK_LEN; ++j) {
                    hand_arr[iter][0] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
                    hand_arr[iter][1] = new Card(CardValueExt.GetCardValueFromInt(1 + (j / 4)), SuitExt.GetSuitFromInt(1 + (j % 4)));
                    ++iter;
                }
            }

            for(int i = 0; i < COMBINATION; ++i) {
                for(int j = 0; j < COMBINATION; ++j) {

                    if(hand_arr[i][0].Equals(hand_arr[j][0]) || hand_arr[i][0].Equals(hand_arr[j][1])
                        || hand_arr[i][1].Equals(hand_arr[j][0]) || hand_arr[i][1].Equals(hand_arr[j][1])) {
                        full_grid[i][j] = -1;
                        continue;
                    }

                    for(int s = 0; s < COMBINATION; ++s) {
                        for(int t = 0; t < COMBINATION; ++t) {
                            for(int u = 0; u < COMBINATION; ++u) {
                                for(int v = 0; v < COMBINATION; ++v) {
                                    for(int w = 0; w < COMBINATION; ++w) {

                                    }
                                }
                            }
                        }
                    }

                }
            }

            for(int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                }
            }

        }
    }
}
