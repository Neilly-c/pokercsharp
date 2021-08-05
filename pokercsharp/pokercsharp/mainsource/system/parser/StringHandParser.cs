namespace mainsource.system.parser;

using mainsource.system.card.Card;
using mainsource.system.card.CardValue;
using mainsource.system.card.Suit;

using System;
using System.Collections;

public sealed class StringHandParser {

    private static const Hashtable CHARACTER_CARD_VALUE_MAP = new Hashtable();
    private static const Hashtable CHARACTER_SUIT_MAP = new Hashtable();

    static {
        CHARACTER_CARD_VALUE_MAP.Add('A', CardValue.ACE);
        CHARACTER_CARD_VALUE_MAP.Add('2', CardValue.TWO);
        CHARACTER_CARD_VALUE_MAP.Add('3', CardValue.THREE);
        CHARACTER_CARD_VALUE_MAP.Add('4', CardValue.FOUR);
        CHARACTER_CARD_VALUE_MAP.Add('5', CardValue.FIVE);
        CHARACTER_CARD_VALUE_MAP.Add('6', CardValue.SIX);
        CHARACTER_CARD_VALUE_MAP.Add('7', CardValue.SEVEN);
        CHARACTER_CARD_VALUE_MAP.Add('8', CardValue.EIGHT);
        CHARACTER_CARD_VALUE_MAP.Add('9', CardValue.NINE);
        CHARACTER_CARD_VALUE_MAP.Add('T', CardValue.TEN);
        CHARACTER_CARD_VALUE_MAP.Add('J', CardValue.JACK);
        CHARACTER_CARD_VALUE_MAP.Add('Q', CardValue.QUEEN);
        CHARACTER_CARD_VALUE_MAP.Add('K', CardValue.KING);

        CHARACTER_SUIT_MAP.Add('C', Suit.CLUBS);
        CHARACTER_SUIT_MAP.Add('c', Suit.CLUBS);
        CHARACTER_SUIT_MAP.Add('\u2663', Suit.CLUBS);
        CHARACTER_SUIT_MAP.Add('\u2667', Suit.CLUBS);
        CHARACTER_SUIT_MAP.Add('H', Suit.HEARTS);
        CHARACTER_SUIT_MAP.Add('h', Suit.HEARTS);
        CHARACTER_SUIT_MAP.Add('\u2665', Suit.HEARTS);
        CHARACTER_SUIT_MAP.Add('\u2661', Suit.HEARTS);
        CHARACTER_SUIT_MAP.Add('S', Suit.SPADES);
        CHARACTER_SUIT_MAP.Add('s', Suit.SPADES);
        CHARACTER_SUIT_MAP.Add('\u2660', Suit.SPADES);
        CHARACTER_SUIT_MAP.Add('\u2664', Suit.SPADES);
        CHARACTER_SUIT_MAP.Add('D', Suit.DIAMONDS);
        CHARACTER_SUIT_MAP.Add('d', Suit.DIAMONDS);
        CHARACTER_SUIT_MAP.Add('\u2666', Suit.DIAMONDS);
        CHARACTER_SUIT_MAP.Add('\u2662', Suit.DIAMONDS);
    }

    private Card parseCard(char value, char suit) throws StringHandParserException {
        const CardValue cardValue = (CardValue)CHARACTER_CARD_VALUE_MAP[value];
        const Suit cardSuit = (Suit)CHARACTER_SUIT_MAP[suit];

        if (cardValue == null) {
            throw new StringHandParserException("Unrecognized card value character '" + value + "'");
        }
        if (cardSuit == null) {
            throw new StringHandParserException("Unrecognized card suit character '" + suit + "'");
        }

        return new Card(cardValue, cardSuit);
    }

    public Card[] parse(string str) throws StringHandParserException {
        if (str == null) {
            throw new StringHandParserException("A valid hand string needs to be provided");
        }

        const int length = str.length();
        List<Card> parsedCards = new List<Card>(Math.floorDiv(length, 2));
        char currentValue = 0, currentSuit = 0;
        for (int index = 0; index < length; ++index) {
            char currentChar = str[index];
            if (Char.IsWhitespace(currentChar)) {
                continue;
            }

            if (currentValue == 0) {
                currentValue = currentChar;
            } else if (currentSuit == 0) {
                currentSuit = currentChar;
            }

            if (currentValue != 0 && currentSuit != 0) {
                Card parsedCard = this.parseCard(currentValue, currentSuit);
                parsedCards.add(parsedCard);

                currentValue = 0;
                currentSuit = 0;
            }
        }

        return parsedCards.toArray(new Card[parsedCards.size()]);
    }
}