using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
using pokercsharp.mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pokercsharp.mainsource.appendix {
    class FinalHandsDict {
        public static Dictionary<int, FinalHand> finalDict = new Dictionary<int, FinalHand>();
        public static int[][][][][] finalDictArr = new int[52][][][][];
        static Card[] card_arr;

        public void Init() {

            FullCardArr fca = new FullCardArr();
            card_arr = fca.GetCardArr();

            /*
			52_C_5 = 2598960 loops.
			52_C_7 = 133784560 loops.
			*/

            int loop = 0;
            HandEvaluator evaluator = new HandEvaluator();

            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 6;

            for (int a = 4; a < Constants.FULL_DECK_LEN; ++a) {
                finalDictArr[a] = new int[a][][][];
                for (int b = 3; b < a; ++b) {
                    finalDictArr[a][b] = new int[b][][];
                    for (int c = 2; c < b; ++c) {
                        finalDictArr[a][b][c] = new int[c][];
                        for (int d = 1; d < c; ++d) {
                            finalDictArr[a][b][c][d] = new int[d];
                            Parallel.For(0, d, option, e => {
                                Card[] cards = new Card[] { card_arr[a], card_arr[b], card_arr[c], card_arr[d], card_arr[e] };      //あとでカード序列を確認すること
                                FinalHand fh = evaluator.Evaluate(cards);
                                finalDictArr[a][b][c][d][e] = fh.GetHashCode();
                                ++loop;
                                /*
                                foreach(Card card in cards) {
                                    File.AppendAllText(@"D:\Csharp\pokercsharp\finalHandsDict.txt", card.ToAbbreviateString());
                                }
                                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGrid_beta.txt",
                                    "," + fh.ToString() + ", " + fh.GetHashCode() +  Environment.NewLine);*/
                                if ((loop / (Constants._52C5 / 100)) - ((loop - 1) / (Constants._52C5 / 100)) != 0) {
                                    Debug.WriteLine(loop / (Constants._52C5 / 100) + "% complete");
                                }
                            });
                        }
                    }
                }
            }

        }

        public static int EvaluateByHash(Card[] cards) {
            int a = cards[0].GetHashCode(),
                b = cards[1].GetHashCode(),
                c = cards[2].GetHashCode(),
                d = cards[3].GetHashCode(),
                e = cards[4].GetHashCode();
            return finalDictArr[a][b][c][d][e];
        }

        public static int EvaluateByHash(params int[] cards_int) {
            return finalDictArr[cards_int[0]][cards_int[1]][cards_int[2]][cards_int[3]][cards_int[4]];
        }
    }
}
