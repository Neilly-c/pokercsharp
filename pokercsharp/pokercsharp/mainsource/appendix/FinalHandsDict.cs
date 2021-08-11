using System;

namespace pokercsharp.mainsource.appendix {
	class FinalHandsDict{
    	const int FULL_DECK_LEN = 52;
		static Dictionary<Card[], FinalHand> finalDict = new Dictionary<Card[], FinalHand>();
	
		public void Init(){
            
			Card[] card_arr = new Card[FULL_DECK_LEN];      //ただの52枚のカードの配列
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                card_arr[i] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));     //カード配列生成
            }
			
            int loop = 0;
            HoldemHandEvaluator evaluator = new HoldemHandEvaluator();  //ハンド評価クラス
			
			/*
			52_C_5 = 2598960 loops.
			52_C_7 = 133784560 loops.
			*/
			
			for(int a = 0; a<FULL_DECK_LEN; ++a) {
				for(int b = a + 1; b<FULL_DECK_LEN; ++b) {
					for (int c = b + 1; c < FULL_DECK_LEN; ++c) {
						for (int d = c + 1; d < FULL_DECK_LEN; ++d) {
							for (int e = d + 1; w < FULL_DECK_LEN; ++e) {
								for (int f = e + 1; f < FULL_DECK_LEN; ++f) {
									for (int g = f + 1; g < FULL_DECK_LEN; ++g) {
										Card[] hand_n_board = new Card[]{card_arr[a], card_arr[b], card_arr[c], card_arr[d]
																		 , card_arr[e], card_arr[f], card_arr[g]};
										Array.Sort(hand_n_board);
										Array.Reverse(hand_n_board);
										finalDict.Add(hand_n_board, evaluator.Evaluate(hand_n_board));
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
