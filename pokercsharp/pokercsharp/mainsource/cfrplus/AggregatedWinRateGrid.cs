using mainsource.system.card;
using mainsource.system.parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    class AggregatedWinRateGrid {
        int[][] full_grid = new int[Constants.COMBINATION][];
        int[][][][] digest_grid = new int[Constants.CARDVALUE_LEN][][][];
        int[][][][] digest_count = new int[Constants.CARDVALUE_LEN][][][];
        string[] hand_arr;

        public AggregatedWinRateGrid() {
            for (int i = 0; i < Constants.COMBINATION; ++i) {
                full_grid[i] = new int[Constants.COMBINATION];
            }
            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                digest_grid[i] = new int[Constants.CARDVALUE_LEN][][];
                digest_count[i] = new int[Constants.CARDVALUE_LEN][][];
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    digest_grid[i][j] = new int[Constants.CARDVALUE_LEN][];
                    digest_count[i][j] = new int[Constants.CARDVALUE_LEN][];
                    for (int k = 0; k < Constants.CARDVALUE_LEN; ++k) {
                        digest_grid[i][j][k] = new int[Constants.CARDVALUE_LEN];
                        digest_count[i][j][k] = new int[Constants.CARDVALUE_LEN];
                    }
                }
            }
        }

        public void Init() {
            using (StreamReader sr = new StreamReader(@"D:\Csharp\pokercsharp\winRateGridExt.txt")) {
                string rawStr = sr.ReadLine();
                rawStr = rawStr.Substring(0, rawStr.LastIndexOf(','));      //末尾の,を外さないと空文字列をintに変換できない
                hand_arr = rawStr.Split(',');
                int row = 0;
                while (!sr.EndOfStream) {
                    string line = sr.ReadLine();
                    full_grid[row] = Array.ConvertAll(line.Split(','), int.Parse);
                    ++row;
                }
            }
            StringHandParser parser = new StringHandParser();
            int[] key = new int[Constants.COMBINATION];
            for (int i = 0; i < Constants.COMBINATION; ++i) {
                Card[] card_arr_p0 = parser.Parse(hand_arr[i]);
                int a = 12 - ((((int)card_arr_p0[0].GetValue()) + 11) % 13);
                int b = 12 - ((((int)card_arr_p0[1].GetValue()) + 11) % 13);
                if (a > b) {
                    Swap(ref a, ref b);
                }
                if (!card_arr_p0[0].GetSuit().Equals(card_arr_p0[1].GetSuit())) {   //offsuitは入れ替え
                    Swap(ref a, ref b);
                }
                for (int j = 0; j < Constants.COMBINATION; ++j) {
                    if(full_grid[i][j] == -1) {
                        continue;
                    }
                    Card[] card_arr_p1 = parser.Parse(hand_arr[j]);
                    int c = 12 - ((((int)card_arr_p1[0].GetValue()) + 11) % 13);
                    int d = 12 - ((((int)card_arr_p1[1].GetValue()) + 11) % 13);
                    if (c > d) {
                        Swap(ref c, ref d);
                    }
                    if (!card_arr_p1[0].GetSuit().Equals(card_arr_p1[1].GetSuit())) {   //offsuitは入れ替え
                        Swap(ref c, ref d);
                    }
                    digest_grid[a][b][c][d] += full_grid[i][j];
                    ++digest_count[a][b][c][d];
                }
            }
            Debug.WriteLine("ReadCSV Finish");
        }

        public int[][][][] Get_digest_grid() {
            return digest_grid;
        }

        public int[][][][] Get_digest_count() {
            return digest_count;
        }

        static void Swap<T>(ref T a, ref T b) {
            var t = a;
            a = b;
            b = t;
        }
    }
}
