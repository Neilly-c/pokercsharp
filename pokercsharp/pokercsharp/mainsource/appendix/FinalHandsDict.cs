using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace pokercsharp.mainsource.appendix {
    class FinalHandsDict {
        const int FULL_DECK_LEN = 52;
        const int HOLDEM_HAND_CARDS = 7;
        const int _52C5 = 2598960;
        const int _52C7 = 133784560;
        static Dictionary<int, FinalHand> finalDict = new Dictionary<int, FinalHand>();
        static Dictionary<int, int> finalHoldemDict = new Dictionary<int, int>();

        public void Init() {

            Card[] card_arr = new Card[FULL_DECK_LEN];      //ただの52枚のカードの配列
            for (int i = 0; i < FULL_DECK_LEN; ++i) {
                card_arr[i] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));     //カード配列生成
            }

            /*
			52_C_5 = 2598960 loops.
			52_C_7 = 133784560 loops.
			*/

            int loop = 0;
            HandEvaluator evaluator = new HandEvaluator();

            for (int a = 0; a < FULL_DECK_LEN; ++a) {
                for (int b = a + 1; b < FULL_DECK_LEN; ++b) {
                    for (int c = b + 1; c < FULL_DECK_LEN; ++c) {
                        for (int d = c + 1; d < FULL_DECK_LEN; ++d) {
                            for (int e = d + 1; e < FULL_DECK_LEN; ++e) {
                                Card[] cards = new Card[] { card_arr[a], card_arr[b], card_arr[c], card_arr[d], card_arr[e] };
                                int hash = GetHashFromCards(cards);
                                finalDict.Add(hash, evaluator.Evaluate(cards));

                                ++loop;
                                if ((loop / (_52C5 / 100)) - ((loop - 1) / (_52C5 / 100)) != 0) {
                                    Debug.WriteLine(loop / (_52C5 / 100) + "% complete");
                                }
                            }
                        }
                    }
                }
            }

            int loop2 = 0;

            for (int a = 0; a < FULL_DECK_LEN; ++a) {
                for (int b = a + 1; b < FULL_DECK_LEN; ++b) {
                    for (int c = b + 1; c < FULL_DECK_LEN; ++c) {
                        for (int d = c + 1; d < FULL_DECK_LEN; ++d) {
                            for (int e = d + 1; e < FULL_DECK_LEN; ++e) {
                                for (int f = e + 1; f < FULL_DECK_LEN; ++f) {
                                    for (int g = f + 1; g < FULL_DECK_LEN; ++g) {
                                        Card[] hand_n_board = new Card[]{card_arr[a], card_arr[b], card_arr[c], card_arr[d]
                                                                         , card_arr[e], card_arr[f], card_arr[g]};
                                        int keyHash = GetHashFromCards(hand_n_board);
                                        FinalHand fh = Evaluate(hand_n_board);
                                        int valueHash = fh.GetHashCode();
                                        finalHoldemDict.Add(keyHash, valueHash);

                                        ++loop2;
                                        if ((loop2 / (_52C7 / 100)) - ((loop2 - 1) / (_52C7 / 100)) != 0) {
                                            Debug.WriteLine(loop2 / (_52C7 / 100) + "% complete");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.WriteLine(finalHoldemDict.Count() + " items complete");

        }

        public FinalHand Evaluate(Card[] cards) {
            FinalHand result = new FinalHand(HandName.HIGH_CARD, new OptionValue(CardValue.TWO));
            Card[] cards_picked = new Card[5];
            for (int i = 0; i < HOLDEM_HAND_CARDS - 1; ++i) {
                for (int j = i + 1; j < HOLDEM_HAND_CARDS; ++j) {
                    int count = 0;
                    for (int k = 0; k < HOLDEM_HAND_CARDS; ++k) {
                        if (k != i && k != j) {
                            cards_picked[count] = cards[k];
                            ++count;
                        }
                    }
                    int hash = GetHashFromCards(cards_picked);
                    FinalHand temp_result = finalDict[hash];
                    if (temp_result.CompareTo(result) > 0) {
                        result = temp_result;
                    }
                }
            }
            return result;
        }

        private int GetHashFromCards(Card[] cards) {
            Array.Sort(cards);
            Array.Reverse(cards);
            int keyHash = 0;
            for (int i = 0; i < cards.Length; ++i) {
                keyHash += (int)cards[i].GetNumber();
                if (i < cards.Length - 1) {
                    keyHash *= 53;
                }
            }
            return keyHash;
        }
    }
}
