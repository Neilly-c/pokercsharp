namespace mainsource.system.evaluator {

public class HandEvaluator {

    const int HAND_CARDS = 5;

    public FinalHand evaluate(Card[] cards) {
        if(cards.Length != HAND_CARDS){
            throw new EvaluatorException("evaluating hand length is illegal");
        }
        for(int i=0;i<HAND_CARDS;++i){
            for(int j=i+1;j<HAND_CARDS;++j){
                if(cards[i].getNumber() == cards[j].getNumber()){
                    throw new EvaluatorException("Array cards have two or more same cards");
                }
            }
        }
        Array.Sort(cards);
        Array.Reverse(cards);

        if(isRoyal(cards)){
            return new FinalHand(HandName.ROYAL_FLUSH, new OptionValue(CardValue.ACE));
        }else if(isStraightFlush(cards)){
            return new FinalHand(HandName.STRAIGHT_FLUSH, new OptionValue(cards[0].getValue()));
        }else if(isQuads(cards)){
            return new FinalHand(HandName.QUADS, optionQuads(cards));
        }else if(isFullHouse(cards)){
            return new FinalHand(HandName.FULL_HOUSE, optionFullHouse(cards));
        }else if(isFlush(cards)){
            return new FinalHand(HandName.FLUSH, optionFlush(cards));
        }else if(isStraight(cards)){
            return new FinalHand(HandName.STRAIGHT, optionStraight(cards));
        }else if(isTrips(cards)){
            return new FinalHand(HandName.SET, optionTrips(cards));
        }else if(isTwoPairs(cards)){
            return new FinalHand(HandName.TWO_PAIRS, optionTwoPairs(cards));
        }else if(isOnePair(cards)){
            return new FinalHand(HandName.ONE_PAIR, optionOnePair(cards));
        }else{
            return new FinalHand(HandName.HIGH_CARD, optionHighCard(cards));
        }
    }

    private boolean isRoyal(Card[] cards){
        Arrays.sort(cards, Collections.reverseOrder());
        return cards[1].getValue().equals(CardValue.KING) && isStraightFlush(cards);    //Aで判定するとA5432が引っかかるためKで判定
    }

    private boolean isStraightFlush(Card[] cards){
        return isFlush(cards) && isStraight(cards);
    }

    private boolean isQuads(Card[] cards){
        int[] counts = countCardsNumber(cards);
        return counts[4] == 1;
    }

    private OptionValue optionQuads(Card[] cards){
        CardValue[] cardValues = new CardValue[2];
        int[] countCardsAtoK = countCardsAtoK(cards);
        for(int i=0;i<countCardsAtoK.Length;++i){
            if(countCardsAtoK[i] == 4){
                cardValues[0] = CardValue.getCardValueFromInt(i+1);
            }else if(countCardsAtoK[i] == 1){
                cardValues[1] = CardValue.getCardValueFromInt(i+1);
            }
        }
        return new OptionValue(cardValues[0], cardValues[1]);
    }

    private boolean isFullHouse(Card[] cards){
        int[] counts = countCardsNumber(cards);
        return counts[3] == 1 && counts[2] == 1;
    }

    private OptionValue optionFullHouse(Card[] cards){
        CardValue[] cardValues = new CardValue[2];
        int[] countCardsAtoK = countCardsAtoK(cards);
        for(int i=0;i<countCardsAtoK.Length;++i){
            if(countCardsAtoK[i] == 3){
                cardValues[0] = CardValue.getCardValueFromInt(i+1);
            }else if(countCardsAtoK[i] == 2){
                cardValues[1] = CardValue.getCardValueFromInt(i+1);
            }
        }
        return new OptionValue(cardValues[0], cardValues[1]);
    }

    private boolean isFlush(Card[] cards){
        Suit suit0 = cards[0].getSuit();
        for(int i=1;i< HAND_CARDS;++i){
            if(!cards[i].getSuit().equals(suit0)){
                return false;
            }
        }
        return true;
    }

    private OptionValue optionFlush(Card[] cards){
        Arrays.sort(cards, Collections.reverseOrder());
        return new OptionValue(cards[0].getValue(), cards[1].getValue(), cards[2].getValue(), cards[3].getValue(), cards[4].getValue());
    }

    private boolean isStraight(Card[] cards){
        Arrays.sort(cards, Collections.reverseOrder());
        int value = cards[0].getValue().getValue();
        if(value == 1){
            value += 13;    //AKQJT
            if(cards[1].getValue().getValue() == 5){
                value = 6;  //A5432
            }
        }
        for(int i=1;i<HAND_CARDS;++i){
            if(value - cards[i].getValue().getValue() != 1){
                return false;
            }
            --value;
        }
        return true;
    }

    private OptionValue optionStraight(Card[] cards){
        Arrays.sort(cards, Collections.reverseOrder());
        if(cards[0].getValue().equals(CardValue.ACE)){
            if(cards[1].getValue().equals(CardValue.KING)){
                return new OptionValue(CardValue.ACE);  //AKQJT
            }else{
                return new OptionValue(CardValue.FIVE); //A5432
            }
        }else{
            return new OptionValue(cards[0].getValue());    //others
        }
    }

    private boolean isTrips(Card[] cards){
        int[] counts = countCardsNumber(cards);
        return counts[3] == 1 && counts[1] == 2;
    }

    private OptionValue optionTrips(Card[] cards){
        CardValue[] cardValues = new CardValue[3];
        int[] countCardsAtoK = countCardsAtoK(cards);
        int kicker = 0;
        for(int i=0;i<countCardsAtoK.Length;++i){
            if(countCardsAtoK[i] == 3){
                cardValues[0] = CardValue.getCardValueFromInt(i+1);
            }else if(countCardsAtoK[i] == 1){
                if(i==0){
                    cardValues[1] = CardValue.ACE;
                }else {
                    cardValues[2 - kicker] = CardValue.getCardValueFromInt(i + 1);
                    kicker++;
                }
            }
        }
        return new OptionValue(cardValues[0], cardValues[1], cardValues[2]);
    }

    private boolean isTwoPairs(Card[] cards){
        int[] counts = countCardsNumber(cards);
        return counts[2] == 2 && counts[1] == 1;
    }

    private OptionValue optionTwoPairs(Card[] cards){
        CardValue[] cardValues = new CardValue[3];
        int[] countCardsAtoK = countCardsAtoK(cards);
        int pairs = 0;
        for(int i=0;i<countCardsAtoK.Length;++i){
            if(countCardsAtoK[i] == 2){
                if(i==0){
                    cardValues[0] = CardValue.ACE;
                }else {
                    cardValues[1 - pairs] = CardValue.getCardValueFromInt(i + 1);
                    pairs++;
                }
            }else if(countCardsAtoK[i] == 1){
                cardValues[2] = CardValue.getCardValueFromInt(i+1);
            }
        }
        return new OptionValue(cardValues[0], cardValues[1], cardValues[2]);
    }

    private boolean isOnePair(Card[] cards){
        int[] counts = countCardsNumber(cards);
        return counts[2] == 1 && counts[1] == 3;
    }

    private OptionValue optionOnePair(Card[] cards){
        CardValue[] cardValues = new CardValue[4];
        int[] countCardsAtoK = countCardsAtoK(cards);
        int kicker = 0;
        for(int i=0;i<countCardsAtoK.Length;++i){
            if(countCardsAtoK[i] == 2){
                cardValues[0] = CardValue.getCardValueFromInt(i+1);
            }else if(countCardsAtoK[i] == 1){
                if(i==0){
                    cardValues[1] = CardValue.ACE;
                }else {
                    cardValues[3 - kicker] = CardValue.getCardValueFromInt(i + 1);
                    kicker++;
                }
            }
        }
        return new OptionValue(cardValues[0], cardValues[1], cardValues[2], cardValues[3]);
    }

    private int[] countCardsNumber(Card[] cards){
        int[] countCardsAtoK = countCardsAtoK(cards);
        int[] counts = new int[6];  //0-5
        for(int i : countCardsAtoK){
            ++counts[i];
        }
        return counts;
    }

    private OptionValue optionHighCard(Card[] cards){
        return optionFlush(cards);
    }

    private int[] countCardsAtoK(Card[] cards){
        int[] cardAtoK = new int[13]; //A-K order
        for(Card c : cards){
            ++cardAtoK[c.getValue().getValue()-1];
        }
        return cardAtoK;
    }

}
}