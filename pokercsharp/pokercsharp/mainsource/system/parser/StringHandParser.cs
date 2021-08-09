namespace mainsource.system.parser {
    using mainsource.system.card;
    using System.Collections.Generic;

    public class StringHandParser {

        private static readonly Dictionary<char, CardValue> CHARACTER_CARD_VALUE_MAP = new Dictionary<char, CardValue>() {
            {'A', CardValue.ACE },
            {'2', CardValue.TWO },
            {'3', CardValue.THREE },
            {'4', CardValue.FOUR },
            {'5', CardValue.FIVE },
            {'6', CardValue.SIX },
            {'7', CardValue.SEVEN },
            {'8', CardValue.EIGHT },
            {'9', CardValue.NINE },
            {'T', CardValue.TEN },
            {'J', CardValue.JACK },
            {'Q', CardValue.QUEEN },
            {'K', CardValue.KING } 
        };

        private static readonly Dictionary<string, Suit> CHARACTER_SUIT_MAP = new Dictionary<string, Suit>() {
            {"C", Suit.CLUBS },
            {"c", Suit.CLUBS },
            {"\u2663", Suit.CLUBS },
            {"\u2667", Suit.CLUBS },
            {"H", Suit.HEARTS },
            {"h", Suit.HEARTS },
            {"\u2665", Suit.HEARTS },
            {"\u2661", Suit.HEARTS },
            {"S", Suit.SPADES },
            {"s", Suit.SPADES },
            {"\u2660", Suit.SPADES },
            {"\u2664", Suit.SPADES },
            {"D", Suit.DIAMONDS },
            {"d", Suit.DIAMONDS },
            {"\u2666", Suit.DIAMONDS },
            {"\u2662", Suit.DIAMONDS }
        };

        private Card ParseCard(char value, char suit) throws StringHandParserException {
            const CardValue cardValue = CHARACTER_CARD_VALUE_MAP[value];
            const Suit cardSuit = CHARACTER_SUIT_MAP[suit];

            if (cardValue == null) {
                throw new StringHandParserException("Unrecognized card value character '" + value + "'");
            }
            if (cardSuit == null) {
                throw new StringHandParserException("Unrecognized card suit character '" + suit + "'");
            }

            return new Card(cardValue, cardSuit);
        }

        public Card[] Parse(string str) throws StringHandParserException {
            if (str == null) {
                throw new StringHandParserException("A valid hand string needs to be provided");
            }

            const int length = str.length();
            List<Card> parsedCards = new List<Card>(Math.floorDiv(length, 2));
            char currentValue = 0, currentSuit = 0;
            for (int index = 0; index < length; ++index){
                char currentChar = str[index];
                if (Char.IsWhitespace(currentChar)){
                    continue;
                }

                if (currentValue == 0){
                    currentValue = currentChar;
                }else if (currentSuit == 0){
                    currentSuit = currentChar;
                }

                if (currentValue != 0 && currentSuit != 0){
                    Card parsedCard = this.parseCard(currentValue, currentSuit);
                    parsedCards.add(parsedCard);

                    currentValue = 0;
                    currentSuit = 0;
                }
            }
            return parsedCards.toArray(new Card[parsedCards.size()]);
        }
    }
}