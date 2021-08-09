namespace mainsource.system.evaluator {

	public class HoldemHandEvaluator : HandEvaluator {

		const int HOLDEM_HAND_CARDS = 7;

		public FinalHand evaluate(params Card[][] cards) {
			int len = 0;
			for (int i = 0; i < cards.Length; ++i) {
				len += cards[i].Length;
			}
			Card[] concatCards = new Card[len];
			len = 0;
			for (int i = 0; i < cards.Length; ++i) {
				Array.Copy(cards[i], 0, concatCards, len, cards[i].Length);
				len += cards[i].Length;
			}
			return evaluate(concatCards);
		}

		public override FinalHand evaluate(Card[] cards) {
			if (cards.Length != HOLDEM_HAND_CARDS) {
				throw new EvaluatorException("evaluating HOLDEM hand Length is illegal");
			}
			for (int i = 0; i < HOLDEM_HAND_CARDS; ++i) {
				for (int j = i + 1; j < HOLDEM_HAND_CARDS; ++j) {
					if (cards[i].getNumber() == cards[j].getNumber()) {
						throw new EvaluatorException("Array cards have two or more same cards");
					}
				}
			}
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
					FinalHand temp_result = super.evaluate(cards_picked);
					if (temp_result.compareTo(result) > 0) {
						result = temp_result;
					}
				}
			}
			return result;
		}

	}
}