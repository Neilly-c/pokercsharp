using System;

namespace pokercsharp.mainsource.appendix {
	class FinalHandsDict{
    	const int FULL_DECK_LEN = 52;
		static Dictionary<Card[], FinalHand> finalDict = new Dictionary<Card[], FinalHand>();
		static Dictionary<int, int> finalHoldemDict = new Dictionary<int, int>();
	
		public void Init(){
            
			Card[] card_arr = new Card[FULL_DECK_LEN];      //ただの52枚のカードの配列
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                card_arr[i] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));     //カード配列生成
            }
			
			/*
			52_C_5 = 2598960 loops.
			52_C_7 = 133784560 loops.
			*/
			
            int loop = 0;
            HandEvaluator evaluator = new HandEvaluator();
			
			for(int a = 0; a<FULL_DECK_LEN; ++a) {
				for(int b = a + 1; b<FULL_DECK_LEN; ++b) {
					for (int c = b + 1; c < FULL_DECK_LEN; ++c) {
						for (int d = c + 1; d < FULL_DECK_LEN; ++d) {
							for (int e = d + 1; w < FULL_DECK_LEN; ++e) {
								Card[] cards = new Card[]{card_arr[a], card_arr[b], card_arr[c], card_arr[d], card_arr[e]};
								Array.Sort(cards);
								Array.Reverse(cards);
								finalDict.Add(cards, evaluator.Evaluate(cards));
								++loop;
								if((loop % 25989) - ((loop - 1) % 25989 != 0){
									Debug.WriteLine(loop % 25989 + "% complete");
								}
							}
						}
					}
				}
			}
                                   
            int loop2 = 0;
            HoldemHandEvaluator h_evaluator = new HoldemHandEvaluator();
			
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
										finalHoldemDict.Add(hand_n_board, h_evaluator.Evaluate(hand_n_board));
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
