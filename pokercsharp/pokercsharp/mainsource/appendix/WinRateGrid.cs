using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokercsharp.mainsource.appendix {
    class WinRateGrid {
        const int COMBINATION = 1326;
        const int FULL_DECK_LEN = 52;
        const int HOLDEM_HAND_CARDS = 7;
        const int COM_2 = 878485;
        const int _48C5 = 1712304;

        public void Init() {
            int[][] full_grid = new int[COMBINATION][];     //1326*1326
            FullCardArr fca = new FullCardArr();
            Card[] card_arr = fca.GetCardArr();             //ただの52枚の配列
            Card[][] hand_arr = new Card[COMBINATION][];    //1326通りのハンドの配列
            int[][] hand_arr_int = new int[COMBINATION][];
            for (int i = 0; i < COMBINATION; ++i) {
                full_grid[i] = new int[COMBINATION];
                hand_arr[i] = new Card[2];                  //配列初期化処理(javaと違って2次元配列はこうやって定義しないといけないらしい)
                hand_arr_int[i] = new int[2];
            }

            int iter = 0;
            for (int i = 0; i < FULL_DECK_LEN; ++i) {
                for (int j = i + 1; j < FULL_DECK_LEN; ++j) {       //ハンド配列生成
                    hand_arr[iter][0] = card_arr[i];
                    hand_arr[iter][1] = card_arr[j];
                    hand_arr_int[iter][0] = i;
                    hand_arr_int[iter][1] = j;
                    ++iter;
                }
            }

            int loop = 0;

            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 6;

            for (int i = 0; i < COMBINATION; ++i) {
                Parallel.For(i + 1, COMBINATION, option, j => {      // 1326 * 1325 / 2 = 878475 loops.

                    if (full_grid[i][j] == 0) {

                        if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1])) {   //持ってるカードが被ってるハンドは除外
                            full_grid[i][j] = -1;
                            full_grid[j][i] = -1;
                            loop += 2;
                        } else {

                            int winCount = 0, count = 0;        //winCount 勝ったハンドは+2，引き分けたハンドは+1する　countは常時+2する
                            for (int a = 4; a < FULL_DECK_LEN; ++a) {
                                if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], a)) {
                                    continue;
                                }
                                for (int b = 3; b < a; ++b) {
                                    if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], b)) {
                                        continue;
                                    }
                                    for (int c = 2; c < b; ++c) {
                                        if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], c)) {
                                            continue;
                                        }
                                        for (int d = 1; d < c; ++d) {
                                            if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], d)) {
                                                continue;
                                            }
                                            for (int e = 1; e < d; ++e) {
                                                if (IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], e)) {
                                                    int f_p1 = Evaluate(hand_arr_int[i][0], hand_arr_int[i][1], a, b, c, d, e),  //ハンドとボードの情報を渡して完成ハンドを返してもらう
                                                        f_p2 = Evaluate(hand_arr_int[j][0], hand_arr_int[j][1], a, b, c, d, e);
                                                    if (f_p1 > f_p2) {  //クラスFinalHandはIComparableを継承しているのでCompareTo()で比較できる 比較大なら勝ち
                                                        winCount += 2;
                                                    } else if (f_p1 == f_p2) {   //イコールなら引き分け
                                                        winCount += 1;
                                                    }
                                                    count += 2;
                                                    if (count / (_48C5 / 5) - (count - 1) / (_48C5 / 5) != 0) {
                                                        Debug.WriteLine(count / (_48C5 / 50) + " % count");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            full_grid[i][j] = winCount;         //集計して勝った回数を書いておく　実際に使う時は分母で割って確率を出せばよい(double型より軽く正確)
                            full_grid[j][i] = count - winCount; //逆のマッチアップは勝率も逆にしておく

                            loop += 2;
                            Debug.WriteLine(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + "-"
                                + hand_arr[j][0].ToAbbreviateString() + hand_arr[j][1].ToAbbreviateString() + ", " +
                                loop + " complete, " + full_grid[i][j] + "-" + full_grid[j][i]);

                            for (int i_ = i; i_ < COMBINATION; ++i_) {       //計算省略できるところを省略するために調査する
                                for (int j_ = i_ + 1; j_ < COMBINATION; ++j_) {      // MAX 1326 * 1325 / 2 = 878475 loops.
                                    if (full_grid[i_][j_] != 0) {     //もう埋まってるところはパス
                                        continue;
                                    }
                                    CardValue
                                        cv_i0 = hand_arr[i][0].GetValue(),
                                        cv_i1 = hand_arr[i][1].GetValue(),
                                        cv_j0 = hand_arr[j][0].GetValue(),
                                        cv_j1 = hand_arr[j][1].GetValue(),
                                        cv_i_0 = hand_arr[i_][0].GetValue(),
                                        cv_i_1 = hand_arr[i_][1].GetValue(),
                                        cv_j_0 = hand_arr[j_][0].GetValue(),
                                        cv_j_1 = hand_arr[j_][1].GetValue();
                                    Suit
                                        s_i0 = hand_arr[i][0].GetSuit(),
                                        s_i1 = hand_arr[i][1].GetSuit(),
                                        s_j0 = hand_arr[j][0].GetSuit(),
                                        s_j1 = hand_arr[j][1].GetSuit(),
                                        s_i_0 = hand_arr[i_][0].GetSuit(),
                                        s_i_1 = hand_arr[i_][1].GetSuit(),
                                        s_j_0 = hand_arr[j_][0].GetSuit(),
                                        s_j_1 = hand_arr[j_][1].GetSuit();
                                    if (!cv_i0.Equals(cv_i_0) || !cv_i1.Equals(cv_i_1) || !cv_j0.Equals(cv_j_0) || !cv_j1.Equals(cv_j_1)) {
                                        continue;   //数字が違うやつは結果が違うのでパス
                                    }
                                    if (s_i0.Equals(s_i1) == s_i_0.Equals(s_i_1)
                                      && s_i0.Equals(s_j0) == s_i_0.Equals(s_j_0)
                                      && s_i0.Equals(s_j1) == s_i_0.Equals(s_j_1)
                                      && s_i1.Equals(s_j0) == s_i_1.Equals(s_j_0)
                                      && s_i1.Equals(s_j1) == s_i_1.Equals(s_j_1)
                                      && s_j0.Equals(s_j1) == s_j_0.Equals(s_j_1)) {     //6通り全部の組み合わせが同じなら同じと見なす これだと若干抜けがあるけど
                                        full_grid[i_][j_] = winCount;
                                        full_grid[j_][i_] = count - winCount;
                                        Debug.WriteLine(hand_arr[i_][0].ToAbbreviateString() + hand_arr[i_][1].ToAbbreviateString() + "-"
                                            + hand_arr[j_][0].ToAbbreviateString() + hand_arr[j_][1].ToAbbreviateString() + ", " +
                                            " same as the former's, " + full_grid[i_][j_] + "-" + full_grid[j_][i_]);
                                        loop += 2;
                                    }
                                }
                            }

                            if (loop / (COM_2 / 100) - (loop - 1) / (COM_2 / 100) != 0) {
                                Debug.WriteLine(loop / (COM_2 / 100) + " % complete");
                            }
                        }
                    }
                });
            }

            for (int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt",
                    hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString());
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                    File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt", ", ");
                } else {
                    File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt", Environment.NewLine);
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt",
                    string.Join(", ", full_grid[i]) + Environment.NewLine);
            }

        }

        public int Evaluate(params int[] cards_int) {
            int[] cards_picked_int = new int[5];
            int result = 0;
            for (int i = 0; i < HOLDEM_HAND_CARDS - 1; ++i) {
                for (int j = i + 1; j < HOLDEM_HAND_CARDS; ++j) {
                    int count = 0;
                    for (int k = 0; k < HOLDEM_HAND_CARDS; ++k) {
                        if (k != i && k != j) {
                            cards_picked_int[count] = cards_int[k];
                            ++count;
                        }
                    }
                    Array.Sort(cards_picked_int);
                    Array.Reverse(cards_picked_int);

                    int temp_result = FinalHandsDict.EvaluateByHash(cards_picked_int);
                    if (temp_result > result) {
                        result = temp_result;
                    }

                }
            }
            return result;
        }

        private bool IsAllDifferent(params int[] ints) {
            for (int i = 0; i < ints.Length; ++i) {
                for (int j = i + 1; j < ints.Length; ++j) {
                    if (ints[i] == ints[j]) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
