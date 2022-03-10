using mainsource.system.card;
using mainsource.system.parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    class WinRateGrid13Reader {
        int[][] full_grid_52 = new int[Constants.COMBINATION][];
        int[][][][] win_count_13 = new int[Constants.CARDVALUE_LEN][][][];
        int[][][][] match_up_count_13 = new int[Constants.CARDVALUE_LEN][][][];
        string[] hand_arr;

        /*
         * hand_arr: 1326通りのハンドの配列 .txtからインポートしている
         * 
         * 
         */

        public WinRateGrid13Reader() {
            for (int i = 0; i < Constants.COMBINATION; ++i) {
                full_grid_52[i] = new int[Constants.COMBINATION];
            }
            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                win_count_13[i] = new int[Constants.CARDVALUE_LEN][][];
                match_up_count_13[i] = new int[Constants.CARDVALUE_LEN][][];
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    win_count_13[i][j] = new int[Constants.CARDVALUE_LEN][];
                    match_up_count_13[i][j] = new int[Constants.CARDVALUE_LEN][];
                    for (int k = 0; k < Constants.CARDVALUE_LEN; ++k) {
                        win_count_13[i][j][k] = new int[Constants.CARDVALUE_LEN];
                        match_up_count_13[i][j][k] = new int[Constants.CARDVALUE_LEN];
                    }
                }
            }
        }

        public void Init() {
            using (StreamReader sr = new StreamReader(@"D:\Csharp\pokercsharp\winRateGridExt.txt")) {
                string rawStr = sr.ReadLine();
                rawStr = rawStr.Substring(0, rawStr.LastIndexOf(','));      //末尾の,を外さないと空文字列をintに変換できない
                hand_arr = rawStr.Split(',');                               //hand_arrを外部ファイルより定義 1326通りのハンドの配列
                int row = 0;
                while (!sr.EndOfStream) {
                    string line = sr.ReadLine();
                    full_grid_52[row] = Array.ConvertAll(line.Split(','), int.Parse);
                    ++row;
                }
            }
            int[] key = new int[Constants.COMBINATION];
            for (int i = 0; i < Constants.COMBINATION; ++i) {
                Card[] card_arr_p0 = StringHandParser.Parse(hand_arr[i]);   //hand_arrからハンドを一つ取得し、カード2枚に分解
                int a = 12 - ((((int)card_arr_p0[0].GetValue()) + 11) % 13);    //23456789TJQKAの順に並び変え
                int b = 12 - ((((int)card_arr_p0[1].GetValue()) + 11) % 13);
                if (a > b) {
                    Swap(ref a, ref b);                                         //suitedかPPは常にa<=bとする
                }
                if (!card_arr_p0[0].GetSuit().Equals(card_arr_p0[1].GetSuit())) {   //offsuitは入れ替え
                    Swap(ref a, ref b);
                }
                for (int j = 0; j < Constants.COMBINATION; ++j) {
                    if(full_grid_52[i][j] == -1) {
                        continue;
                    }
                    Card[] card_arr_p1 = StringHandParser.Parse(hand_arr[j]);
                    int c = 12 - ((((int)card_arr_p1[0].GetValue()) + 11) % 13);
                    int d = 12 - ((((int)card_arr_p1[1].GetValue()) + 11) % 13);
                    if (c > d) {
                        Swap(ref c, ref d);
                    }
                    if (!card_arr_p1[0].GetSuit().Equals(card_arr_p1[1].GetSuit())) {   //offsuitは入れ替え
                        Swap(ref c, ref d);
                    }
                    win_count_13[a][b][c][d] += full_grid_52[i][j];     //abがcdに勝っている組み合わせ数
                    ++match_up_count_13[a][b][c][d];                     //ab vs cdの組み合わせ数
                }
            }
            Debug.WriteLine("ReadCSV Finish");
        }

        public int[][][][] Get_win_count_13() {
            /*
             * abがcdに勝っている組み合わせ数を返す
             * a<b(c<d)の時suited、a>b(c>d)の時offsuit
             */
            return win_count_13;
        }

        public int[][][][] Get_match_up_count_13() {
            /*
             * ab vs cdの組み合わせ数を返す
             * a<b(c<d)の時suited、a>b(c>d)の時offsuit
             */
            return match_up_count_13;
        }

        static void Swap<T>(ref T a, ref T b) {
            var t = a;
            a = b;
            b = t;
        }
    }
}
