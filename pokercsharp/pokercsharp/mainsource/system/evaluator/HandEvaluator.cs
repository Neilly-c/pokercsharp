using mainsource.system.card;
using mainsource.system.handvalue;
using System;

namespace mainsource.system.evaluator {

    public class HandEvaluator {

        const int HAND_CARDS = 5;

        public FinalHand Evaluate(Card[] cards) {
            if (cards.Length != HAND_CARDS) {
                throw new EvaluatorException("evaluating hand length is illegal");
            }
            for (int i = 0; i < HAND_CARDS; ++i) {
                for (int j = i + 1; j < HAND_CARDS; ++j) {
                    if (cards[i].GetNumber() == cards[j].GetNumber()) {
                        throw new EvaluatorException("Array cards have two or more same cards");
                    }
                }
            }
            Array.Sort(cards);
            Array.Reverse(cards);

            if (IsRoyal(cards)) {
                return new FinalHand(HandName.ROYAL_FLUSH, new OptionValue(CardValue.ACE));
            } else if (IsStraightFlush(cards)) {
                return new FinalHand(HandName.STRAIGHT_FLUSH, new OptionValue(cards[0].GetValue()));
            } else if (IsQuads(cards)) {
                return new FinalHand(HandName.QUADS, OptionQuads(cards));
            } else if (IsFullHouse(cards)) {
                return new FinalHand(HandName.FULL_HOUSE, OptionFullHouse(cards));
            } else if (IsFlush(cards)) {
                return new FinalHand(HandName.FLUSH, OptionFlush(cards));
            } else if (IsStraight(cards)) {
                return new FinalHand(HandName.STRAIGHT, OptionStraight(cards));
            } else if (IsTrips(cards)) {
                return new FinalHand(HandName.SET, OptionTrips(cards));
            } else if (IsTwoPairs(cards)) {
                return new FinalHand(HandName.TWO_PAIRS, OptionTwoPairs(cards));
            } else if (IsOnePair(cards)) {
                return new FinalHand(HandName.ONE_PAIR, OptionOnePair(cards));
            } else {
                return new FinalHand(HandName.HIGH_CARD, OptionHighCard(cards));
            }
        }

        private bool IsRoyal(Card[] cards) {
            Array.Sort(cards);
            Array.Reverse(cards);
            return cards[1].GetValue().Equals(CardValue.KING) && IsStraightFlush(cards);    //Aで判定するとA5432が引っかかるためKで判定
        }

        private bool IsStraightFlush(Card[] cards) {
            return IsFlush(cards) && IsStraight(cards);
        }

        private bool IsQuads(Card[] cards) {
            int[] counts = CountCardsNumber(cards);
            return counts[4] == 1;
        }

        private OptionValue OptionQuads(Card[] cards) {
            CardValue[] cardValues = new CardValue[2];
            int[] countCardsAtoK = CountCardsAtoK(cards);
            for (int i = 0; i < countCardsAtoK.Length; ++i) {
                if (countCardsAtoK[i] == 4) {
                    cardValues[0] = CardValueExt.GetCardValueFromInt(i + 1);
                } else if (countCardsAtoK[i] == 1) {
                    cardValues[1] = CardValueExt.GetCardValueFromInt(i + 1);
                }
            }
            return new OptionValue(cardValues[0], cardValues[1]);
        }

        private bool IsFullHouse(Card[] cards) {
            int[] counts = CountCardsNumber(cards);
            return counts[3] == 1 && counts[2] == 1;
        }

        private OptionValue OptionFullHouse(Card[] cards) {
            CardValue[] cardValues = new CardValue[2];
            int[] countCardsAtoK = CountCardsAtoK(cards);
            for (int i = 0; i < countCardsAtoK.Length; ++i) {
                if (countCardsAtoK[i] == 3) {
                    cardValues[0] = CardValueExt.GetCardValueFromInt(i + 1);
                } else if (countCardsAtoK[i] == 2) {
                    cardValues[1] = CardValueExt.GetCardValueFromInt(i + 1);
                }
            }
            return new OptionValue(cardValues[0], cardValues[1]);
        }

        private bool IsFlush(Card[] cards) {
            Suit suit0 = cards[0].GetSuit();
            for (int i = 1; i < HAND_CARDS; ++i) {
                if (!cards[i].GetSuit().Equals(suit0)) {
                    return false;
                }
            }
            return true;
        }

        private OptionValue OptionFlush(Card[] cards)
        {
            Array.Sort(cards);
            Array.Reverse(cards);
            return new OptionValue(cards[0].GetValue(), cards[1].GetValue(), cards[2].GetValue(), cards[3].GetValue(), cards[4].GetValue());
        }

        private bool IsStraight(Card[] cards)
        {
            Array.Sort(cards);
            Array.Reverse(cards);
            int value = cards[0].GetValue().GetValue();
            if (value == 1) {
                value += 13;    //AKQJT
                if (cards[1].GetValue().GetValue() == 5) {
                    value = 6;  //A5432
                }
            }
            for (int i = 1; i < HAND_CARDS; ++i) {
                if (value - cards[i].GetValue().GetValue() != 1) {
                    return false;
                }
                --value;
            }
            return true;
        }

        private OptionValue OptionStraight(Card[] cards)
        {
            Array.Sort(cards);
            Array.Reverse(cards);
            if (cards[0].GetValue().Equals(CardValue.ACE)) {
                if (cards[1].GetValue().Equals(CardValue.KING)) {
                    return new OptionValue(CardValue.ACE);  //AKQJT
                } else {
                    return new OptionValue(CardValue.FIVE); //A5432
                }
            } else {
                return new OptionValue(cards[0].GetValue());    //others
            }
        }

        private bool IsTrips(Card[] cards) {
            int[] counts = CountCardsNumber(cards);
            return counts[3] == 1 && counts[1] == 2;
        }

        private OptionValue OptionTrips(Card[] cards) {
            CardValue[] cardValues = new CardValue[3];
            int[] countCardsAtoK = CountCardsAtoK(cards);
            int kicker = 0;
            for (int i = 0; i < countCardsAtoK.Length; ++i) {
                if (countCardsAtoK[i] == 3) {
                    cardValues[0] = CardValueExt.GetCardValueFromInt(i + 1);
                } else if (countCardsAtoK[i] == 1) {
                    if (i == 0) {
                        cardValues[1] = CardValue.ACE;
                    } else {
                        cardValues[2 - kicker] = CardValueExt.GetCardValueFromInt(i + 1);
                        kicker++;
                    }
                }
            }
            return new OptionValue(cardValues[0], cardValues[1], cardValues[2]);
        }

        private bool IsTwoPairs(Card[] cards) {
            int[] counts = CountCardsNumber(cards);
            return counts[2] == 2 && counts[1] == 1;
        }

        private OptionValue OptionTwoPairs(Card[] cards) {
            CardValue[] cardValues = new CardValue[3];
            int[] countCardsAtoK = CountCardsAtoK(cards);
            int pairs = 0;
            for (int i = 0; i < countCardsAtoK.Length; ++i) {
                if (countCardsAtoK[i] == 2) {
                    if (i == 0) {
                        cardValues[0] = CardValue.ACE;
                    } else {
                        cardValues[1 - pairs] = CardValueExt.GetCardValueFromInt(i + 1);
                        pairs++;
                    }
                } else if (countCardsAtoK[i] == 1) {
                    cardValues[2] = CardValueExt.GetCardValueFromInt(i + 1);
                }
            }
            return new OptionValue(cardValues[0], cardValues[1], cardValues[2]);
        }

        private bool IsOnePair(Card[] cards) {
            int[] counts = CountCardsNumber(cards);
            return counts[2] == 1 && counts[1] == 3;
        }

        private OptionValue OptionOnePair(Card[] cards) {
            CardValue[] cardValues = new CardValue[4];
            int[] countCardsAtoK = CountCardsAtoK(cards);
            int kicker = 0;
            for (int i = 0; i < countCardsAtoK.Length; ++i) {
                if (countCardsAtoK[i] == 2) {
                    cardValues[0] = CardValueExt.GetCardValueFromInt(i + 1);
                } else if (countCardsAtoK[i] == 1) {
                    if (i == 0) {
                        cardValues[1] = CardValue.ACE;
                    } else {
                        cardValues[3 - kicker] = CardValueExt.GetCardValueFromInt(i + 1);
                        kicker++;
                    }
                }
            }
            return new OptionValue(cardValues[0], cardValues[1], cardValues[2], cardValues[3]);
        }

        private int[] CountCardsNumber(Card[] cards) {
            int[] countCardsAtoK = CountCardsAtoK(cards);
            int[] counts = new int[6];  //0-5
            foreach (int i in countCardsAtoK) {
                ++counts[i];
            }
            return counts;
        }

        private OptionValue OptionHighCard(Card[] cards) {
            return OptionFlush(cards);
        }

        private int[] CountCardsAtoK(Card[] cards) {
            int[] cardAtoK = new int[13]; //A-K order
            foreach (Card c in cards) {
                ++cardAtoK[c.GetValue().GetValue() - 1];
            }
            return cardAtoK;
        }

    }
}