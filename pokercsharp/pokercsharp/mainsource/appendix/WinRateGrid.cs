using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
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
            Card[] card_arr = new Card[FULL_DECK_LEN];
            Card[][] hand_arr = new Card[COMBINATION][];
            for (int i = 0; i < COMBINATION; ++i) {
                full_grid[i] = new int[COMBINATION];
                hand_arr[i] = new Card[2];
            }
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                card_arr[i] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
            }

            int iter = 0;
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                for (int j = i + 1; j < FULL_DECK_LEN; ++j) {
                    hand_arr[iter][0] = card_arr[i];
                    hand_arr[iter][1] = card_arr[j];
                    ++iter;
                }
            }

            int loop = 0;

            for(int i = 0; i < COMBINATION; ++i) {
                for(int j = i + 1; j < COMBINATION; ++j) {

                    int p1_0 = 0, p1_1 = 0, p2_0 = 0, p2_1 = 0;
                    for(int k = 0; k < FULL_DECK_LEN; ++k) {
                        if (card_arr[k].Equals(hand_arr[i][0])) {
                            p1_0 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[i][1])) {
                            p1_1 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[j][0])) {
                            p2_0 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[j][1])) {
                            p2_1 = k;
                        }
                    }
                    if(!IsAllDifferent(p1_0, p1_1, p2_0, p2_1) ){
                        full_grid[i][j] = -1;
                        full_grid[j][i] = -1;
                        continue;
                    }

                    HoldemHandEvaluator evaluator = new HoldemHandEvaluator();
                    int winCount = 0, count = 0;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for(int s = 0; s < FULL_DECK_LEN; ++s) {
                        if(!IsAllDifferent(p1_0, p1_1, p2_0, p2_1, s)) {
                            continue;
                        }
                        for(int t = s + 1; t < FULL_DECK_LEN; ++t) {
                            if (!IsAllDifferent(p1_0, p1_1, p2_0, p2_1, t)) {
                                continue;
                            }
                            for (int u = t + 1; u < FULL_DECK_LEN; ++u) {
                                if (!IsAllDifferent(p1_0, p1_1, p2_0, p2_1, u)) {
                                    continue;
                                }
                                for (int v = u + 1; v < FULL_DECK_LEN; ++v) {
                                    if (!IsAllDifferent(p1_0, p1_1, p2_0, p2_1, v)) {
                                        continue;
                                    }
                                    for (int w = v + 1; w < FULL_DECK_LEN; ++w) {
                                        if (!IsAllDifferent(p1_0, p1_1, p2_0, p2_1, w)) {
                                            continue;
                                        }
                                        Card[] board = new Card[] { card_arr[s], card_arr[t], card_arr[u], card_arr[v], card_arr[w] };
                                        FinalHand f_p1 = evaluator.Evaluate(hand_arr[i], board),
                                            f_p2 = evaluator.Evaluate(hand_arr[j], board);
                                        if(f_p1.CompareTo(f_p2) > 0) {
                                            winCount += 2;
                                        }else if(f_p1.CompareTo(f_p2) == 0) {
                                            winCount += 1;
                                        }
                                        count += 2;
                                        if(count % 20000 == 19998) {
                                            sw.Stop();
                                            Debug.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
                                            sw.Start();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    full_grid[i][j] = winCount;
                    full_grid[j][i] = count - winCount;

                    ++loop;
                    Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + "-"
                        + hand_arr[j][0].ToAbbreviateString() + hand_arr[j][1].ToAbbreviateString() + ", " + 
                        loop + " complete, " + full_grid[i][j] + "-" + full_grid[j][i]);
                }
            }

            for(int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                Debug.WriteLine(string.Join(", ", full_grid[i]));
            }

        }

        private bool IsAllDifferent(params int[] ints) {
            for(int i = 0; i < ints.Length; ++i) {
                for(int j = i + 1; j < ints.Length; ++j) {
                    if(ints[i] == ints[j]) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
