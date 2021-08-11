using System;

namespace pokercsharp.mainsource.appendix {
	class FinalHandsDict{
    	const int FULL_DECK_LEN = 52;
		static Dictionary<Card[], FinalHand> finalDict = new Dictionary<Card[], FinalHand>();
	
		public void Init(){
            Card[] card_arr = new Card[FULL_DECK_LEN];      //ただの52枚のカードの配列
			for(int s = 0; s < card_list_edit.Length; ++s) {
				for(int t = s + 1; t < card_list_edit.Length; ++t) {
					for (int u = t + 1; u < card_list_edit.Length; ++u) {
						for (int v = u + 1; v < card_list_edit.Length; ++v) {
							for (int w = v + 1; w < card_list_edit.Length; ++w) {       //これなんとかならんの？？？
								Card[] board = new Card[] { card_list_edit[s], card_list_edit[t], card_list_edit[u], card_list_edit[v], card_list_edit[w] 
							}
						}
					}
				}
			}
		}
	}
}
