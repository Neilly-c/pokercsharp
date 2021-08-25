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

            //intList_for_compute.Clear();
            //intList_for_compute.Add(585);

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
                                loop + " complete, " + full_grid[i][j] + "-" + full_grid[j][i] + " (" + winCount + " wins, " + evenCount + " ties in count of " + count + ")");


                            CardValue
                                cv_i0 = hand_arr[i][0].GetValue(),
                                cv_i1 = hand_arr[i][1].GetValue(),
                                cv_j0 = hand_arr[j][0].GetValue(),
                                cv_j1 = hand_arr[j][1].GetValue();
                            Suit
                                s_i0 = hand_arr[i][0].GetSuit(),
                                s_i1 = hand_arr[i][1].GetSuit(),
                                s_j0 = hand_arr[j][0].GetSuit(),
                                s_j1 = hand_arr[j][1].GetSuit();
                            for (int x = i; x < COMBINATION; ++x) {       //同じ結果になるところを埋める
                                CardValue
                                    cv_x0 = hand_arr[x][0].GetValue(),
                                    cv_x1 = hand_arr[x][1].GetValue();
                                Suit
                                    s_x0 = hand_arr[x][0].GetSuit(),
                                    s_x1 = hand_arr[x][1].GetSuit();
                                if(!((cv_i0.Equals(cv_x0) && cv_i1.Equals(cv_x1)) || (cv_i0.Equals(cv_x1) && cv_i1.Equals(cv_x0)))){
                                    continue;       //数字が違うやつはパス
                                }
                                for (int y = x + 1; y < COMBINATION; ++y) {      // MAX 1326 * 1325 / 2 = 878475 loops.
                                    if (full_grid[x][y] != 0) {     //もう埋まってるところはパス
                                        continue;
                                    }
                                    CardValue
                                        cv_y0 = hand_arr[y][0].GetValue(),
                                        cv_y1 = hand_arr[y][1].GetValue();
                                    Suit
                                        s_y0 = hand_arr[y][0].GetSuit(),
                                        s_y1 = hand_arr[y][1].GetSuit();
                                    if (cv_j0.Equals(cv_y0) && cv_j1.Equals(cv_y1)) {
                                        if (s_i0.Equals(s_i1) == s_x0.Equals(s_x1)
                                          && s_i0.Equals(s_j0) == s_x0.Equals(s_y0)
                                          && s_i0.Equals(s_j1) == s_x0.Equals(s_y1)
                                          && s_i1.Equals(s_j0) == s_x1.Equals(s_y0)
                                          && s_i1.Equals(s_j1) == s_x1.Equals(s_y1)
                                          && s_j0.Equals(s_j1) == s_y0.Equals(s_y1)) {     //6通り全部の組み合わせが同じなら同じと見なす
                                            full_grid[x][y] = full_grid[i][j];
                                            full_grid[y][x] = full_grid[j][i];
                                            Debug.WriteLine(hand_arr[x][0].ToAbbreviateString() + hand_arr[x][1].ToAbbreviateString() + "-"
                                                + hand_arr[y][0].ToAbbreviateString() + hand_arr[y][1].ToAbbreviateString() + ", " +
                                                " same as the former's, " + full_grid[x][y] + "-" + full_grid[y][x]);
                                            ++loop;
                                        }
                                    }else if (cv_j0.Equals(cv_y1) && cv_j1.Equals(cv_y0)) {
                                        if (s_i0.Equals(s_i1) == s_x0.Equals(s_x1)
                                          && s_i0.Equals(s_j0) == s_x0.Equals(s_y1)
                                          && s_i0.Equals(s_j1) == s_x0.Equals(s_y0)
                                          && s_i1.Equals(s_j0) == s_x1.Equals(s_y1)
                                          && s_i1.Equals(s_j1) == s_x1.Equals(s_y0)
                                          && s_j0.Equals(s_j1) == s_y1.Equals(s_y0)) {     //y0とy1が逆のやつ
                                            full_grid[x][y] = full_grid[i][j];
                                            full_grid[y][x] = full_grid[j][i];
                                            Debug.WriteLine(hand_arr[x][0].ToAbbreviateString() + hand_arr[x][1].ToAbbreviateString() + "-"
                                                + hand_arr[y][0].ToAbbreviateString() + hand_arr[y][1].ToAbbreviateString() + ", " +
                                                " same as the former's, " + full_grid[x][y] + "-" + full_grid[y][x]);
                                            ++loop;
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
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
