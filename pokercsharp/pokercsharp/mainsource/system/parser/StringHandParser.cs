namespace mainsource.system.parser {
    using mainsource.system.card;
    using System;
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

        private static readonly Dictionary<char, Suit> CHARACTER_SUIT_MAP = new Dictionary<char, Suit>() {
            {'C', Suit.CLUBS },
            {'c', Suit.CLUBS },
            {'H', Suit.HEARTS },
            {'h', Suit.HEARTS },
            {'S', Suit.SPADES },
            {'s', Suit.SPADES },
            {'D', Suit.DIAMONDS },
            {'d', Suit.DIAMONDS }
        };

        private Card ParseCard(char value, char suit){
            CardValue cardValue = CHARACTER_CARD_VALUE_MAP[value];
            Suit cardSuit = CHARACTER_SUIT_MAP[suit];

            return new Card(cardValue, cardSuit);
        }

        public Card[] Parse(string str){
            if (str == null) {
                throw new StringHandParserException("A valid hand string needs to be provided");
            }

            int length = str.Length;
            List<Card> parsedCardList = new List<Card>(length/2);
            char currentValue = '0', currentSuit = '0';
            for (int index = 0; index < length; ++index){
                char currentChar = str[index];
                if (Char.IsWhiteSpace(currentChar)){
                    continue;
                }

                if (currentValue == '0'){
                    currentValue = currentChar;
                }else if (currentSuit == '0'){
                    currentSuit = currentChar;
                }

                if (currentValue != '0' && currentSuit != '0'){
                    Card parsedCard = this.ParseCard(currentValue, currentSuit);
                    parsedCardList.Add(parsedCard);

                    currentValue = '0';
                    currentSuit = '0';
                }
            }
            return parsedCardList.ToArray();
        }
    }
}