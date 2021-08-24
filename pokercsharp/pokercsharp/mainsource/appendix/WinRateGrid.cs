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
        const int ABB_COM = 169;
        const int FULL_DECK_LEN = 52;
        const int HOLDEM_HAND_CARDS = 7;
        const int COM_2 = 878485;
        const int _48C5 = 1712304;

        int[][] full_grid = new int[COMBINATION][];     //1326*1326
        FullCardArr fca = new FullCardArr();
        Card[] card_arr;
        Card[][] hand_arr = new Card[COMBINATION][];    //1326通りのハンドの配列
        int[][] hand_arr_int = new int[COMBINATION][];
        List<int> intList_for_compute = new List<int>(); //ハンドの対称性を考慮して計算が必要なハンドの番号
        int[][][] valSwap = new int[2][][];        //[0]:suited, [1]:offsuit, pair

        public void Init() {
            card_arr = fca.GetCardArr();             //ただの52枚の配列
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
                    if (card_arr[i].GetValue().Equals(card_arr[j].GetValue())) {    //ポケットペア
                        if (card_arr[i].GetSuit().Equals(Suit.CLUBS) && card_arr[j].GetSuit().Equals(Suit.DIAMONDS)) {
                            intList_for_compute.Add(iter);
                        }
                    } else {    //ペア以外
                        if (card_arr[i].GetSuit().Equals(Suit.CLUBS) && card_arr[i].GetValue() > card_arr[j].GetValue()) {
                            if (card_arr[j].GetSuit().Equals(Suit.DIAMONDS)) {      //オフスート
                                intList_for_compute.Add(iter);
                            }
                            if (card_arr[j].GetSuit().Equals(Suit.CLUBS)) {     //スーテッド
                                intList_for_compute.Add(iter);
                            }
                        }
                    }
                    ++iter;
                }
            }

            Debug.WriteLine("size of intList_needs_compute = " + intList_for_compute.Count() + " (169)");

            intList_for_compute.Clear();
            intList_for_compute.Add(585);

            int loop = 0;

            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 6;

            foreach (int i in intList_for_compute) {
                Parallel.For(0, COMBINATION, option, j => {      // 1326 * 1325 / 2 = 878475 -> 169 * 1326 = 224094 loops.
                    ++loop;

                    if (full_grid[i][j] == 0) {

                        if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1])) {   //持ってるカードが被ってるハンドは除外
                            full_grid[i][j] = -1;
                            full_grid[j][i] = -1;
                        } else {

                            int winCount = 0, evenCount = 0, count = 0;        //winCount 勝ったハンドは+2，引き分けたハンドは+1する　countは常時+2する
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
                                            for (int e = 0; e < d; ++e) {
                                                if (IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1], e)) {
                                                    int f_p1 = Evaluate(hand_arr_int[i][0], hand_arr_int[i][1], a, b, c, d, e),  //ハンドとボードの情報を渡して完成ハンドを返してもらう
                                                        f_p2 = Evaluate(hand_arr_int[j][0], hand_arr_int[j][1], a, b, c, d, e);
                                                    if (f_p1 > f_p2) {  //比較大なら勝ち
                                                        ++winCount;
                                                    } else if (f_p1 == f_p2) {   //イコールなら引き分け
                                                        ++evenCount;
                                                    }
                                                    ++count;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            full_grid[i][j] = winCount * 2 + evenCount;         //集計して勝った回数を書いておく　実際に使う時は分母で割って確率を出せばよい(double型より軽く正確)
                            full_grid[j][i] = count * 2 - full_grid[i][j]; //逆のマッチアップは勝率も逆にしておく

                            Debug.WriteLine(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + "-"
                                + hand_arr[j][0].ToAbbreviateString() + hand_arr[j][1].ToAbbreviateString() + ", " +
                                loop + " complete, " + full_grid[i][j] + "-" + full_grid[j][i] + " (" + winCount + ", " + evenCount + ", " + count + ")");
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
                                        ++loop;
                                    }
                                }
                            }
                        }
                    }
                });
                Debug.WriteLine("Line " + i + " complete");
            }

            for (int i = 0; i < COMBINATION; ++i) {
                for (int j = 0; j < COMBINATION; ++j) {
                    if (full_grid[i][j] == 0) {
                        if (full_grid[j][i] != 0) {
                            full_grid[i][j] = full_grid[j][i];
                            continue;
                        }
                        if (!IsAllDifferent(hand_arr_int[i][0], hand_arr_int[i][1], hand_arr_int[j][0], hand_arr_int[j][1])) {   //持ってるカードが被ってるハンドは除外
                            full_grid[i][j] = -1;
                            full_grid[j][i] = -1;
                            continue;
                        } else {
                            Card pl0 = hand_arr[i][0], pl1 = hand_arr[i][1], op0 = hand_arr[j][0], op1 = hand_arr[j][1];

                        }
                    }
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt",
                    hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ", ");
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                    File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt", Environment.NewLine);
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid.txt",
                    "{ " + string.Join(", ", full_grid[i]) + " }," + Environment.NewLine);
            }

            FillSymmetry();

        }

        public void FillSymmetry() {

            for (int i = 0; i < 2; ++i) {
                valSwap[i] = new int[13][];
                for (int j = 0; j < 13; ++j) {
                    valSwap[i][j] = new int[13];
                }
            }

            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 6;

            int iter = 0;
            Parallel.For (0, FULL_DECK_LEN, option, i => {
                for (int j = i + 1; j < FULL_DECK_LEN; ++j) {       //ハンド配列生成
                    hand_arr[iter][0] = card_arr[i];
                    hand_arr[iter][1] = card_arr[j];
                    hand_arr_int[iter][0] = i;
                    hand_arr_int[iter][1] = j;
                    if (card_arr[i].GetValue().Equals(card_arr[j].GetValue())) {    //ポケットペア
                        if (card_arr[i].GetSuit().Equals(Suit.CLUBS) && card_arr[j].GetSuit().Equals(Suit.DIAMONDS)) {
                            valSwap[1][(int)card_arr[i].GetValue() - 1][(int)card_arr[j].GetValue() - 1] = iter;
                        }
                    } else {    //ペア以外
                        if (card_arr[i].GetSuit().Equals(Suit.CLUBS) && card_arr[i].GetValue() > card_arr[j].GetValue()) {
                            if (card_arr[j].GetSuit().Equals(Suit.DIAMONDS)) {      //オフスート
                                valSwap[1][(int)card_arr[i].GetValue() - 1][(int)card_arr[j].GetValue() - 1] = iter;
                                valSwap[1][(int)card_arr[j].GetValue() - 1][(int)card_arr[i].GetValue() - 1] = iter;
                            }
                            if (card_arr[j].GetSuit().Equals(Suit.CLUBS)) {     //スーテッド
                                valSwap[0][(int)card_arr[i].GetValue() - 1][(int)card_arr[j].GetValue() - 1] = iter;
                                valSwap[0][(int)card_arr[j].GetValue() - 1][(int)card_arr[i].GetValue() - 1] = iter;
                            }
                        }
                    }
                    ++iter;
                }
            });

            int[][][] suitSwap = new int[][][]       //p0の持ってるスーツの組み合わせに応じてどう組み替えるか[p0の1枚目のスート][p0の2枚目のスート][組み替えるスート-1]
            { new int[][]{ new int[]{ 1, 2, 3, 4 }, new int[]{ 1, 2, 3, 4 }, new int[]{ 1, 3, 2, 4 }, new int[]{ 1, 4, 3, 2 } },
             new int[][]{ new int[]{ 2, 1, 3, 4 }, new int[]{ 2, 1, 3, 4 }, new int[]{ 2, 3, 1, 4 }, new int[]{ 2, 4, 3, 1 } },
             new int[][]{ new int[]{ 3, 1, 2, 4 }, new int[]{ 3, 2, 1, 4 }, new int[]{ 3, 2, 1, 4 }, new int[]{ 3, 4, 1, 2 } },
             new int[][]{ new int[]{ 4, 1, 3, 2 }, new int[]{ 4, 2, 3, 1 }, new int[]{ 4, 3, 2, 1 }, new int[]{ 4, 2, 3, 1 } } };
            /*
             * cc
             * cd
             * ch -> chds(d,h)
             * cs -> cshd(d,s)
             * dc -> dchs(c,d)
             * dd -> dchs(c,d)
             * dh -> dhcs(c,d)(c,h)
             * ds -> dshc(c,d)(c,s)
             * hc -> hcds(c,d)(d,h)
             * hd -> hdcs(h,s)
             * hh -> hdcs(c,h)
             * hs -> hscd(c,h)(d,s)
             * sc -> schd(c,d)(d,s)
             * sd -> sdhc(c,s)
             * sh -> shdc(c,s)(d,h)
             * ss -> sdhc(c,s)
             */

            Parallel.For(0, COMBINATION, option, i => {
                for (int j = 0; j < COMBINATION; ++j) {
                    if (full_grid[i][j] != 0) {
                        continue;
                    }
                    if (i == j) {
                        full_grid[i][j] = -1;
                        continue;
                    }
                    if (full_grid[j][i] != 0) {
                        full_grid[i][j] = full_grid[j][i];
                        continue;
                    }
                    Card plr0 = hand_arr[i][0], plr1 = hand_arr[i][1], vln0 = hand_arr[j][0], vln1 = hand_arr[j][1];
                    int x = valSwap[plr0.GetSuit().Equals(plr1.GetSuit()) ? 0 : 1][(int)plr0.GetValue() - 1][(int)plr1.GetValue() - 1]; //参照先x
                    vln0 = new Card(vln0.GetValue(), (Suit)suitSwap[(int)plr0.GetSuit() - 1][(int)plr1.GetSuit() - 1][(int)vln0.GetSuit() - 1]);
                    vln1 = new Card(vln1.GetValue(), (Suit)suitSwap[(int)plr0.GetSuit() - 1][(int)plr1.GetSuit() - 1][(int)vln1.GetSuit() - 1]);
                    int y = 0;
                    for (; y < COMBINATION; ++y) {
                        if ((hand_arr[y][0].Equals(vln0) && hand_arr[y][1].Equals(vln1)) || (hand_arr[y][0].Equals(vln1) && hand_arr[y][1].Equals(vln0))) {    //参照先y
                            full_grid[i][j] = full_grid[x][y];
                            break;
                        }
                    }
                    if (y == COMBINATION) {
                        Debug.WriteLine("Missing hand: " + vln0.ToAbbreviateString() + vln1.ToAbbreviateString());
                    } else {
                        Debug.WriteLine(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + "-"
                             + hand_arr[j][0].ToAbbreviateString() + hand_arr[j][1].ToAbbreviateString() + ", overwritten by "
                             + hand_arr[x][0].ToAbbreviateString() + hand_arr[x][1].ToAbbreviateString() + "-"
                             + hand_arr[y][0].ToAbbreviateString() + hand_arr[y][1].ToAbbreviateString() + " (" + full_grid[i][j] + ")");
                    }
                }
                Debug.WriteLine(i + " complete");
            });
            /*
            for (int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid_beta.txt",
                    hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ", ");
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                    File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid_beta.txt", Environment.NewLine);
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid_beta.txt",
                    string.Join(", ", full_grid[i]) + Environment.NewLine);
            }
            */
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
