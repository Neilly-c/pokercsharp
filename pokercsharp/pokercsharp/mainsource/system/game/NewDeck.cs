using mainsource.system.card;
using pokercsharp.mainsource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mainsource.system.game {

    public class NewDeck {

        private Queue<Card> deck = new Queue<Card>();

        public NewDeck() {
            Initdeck();
        }

        private Queue<Card> Initdeck() {
            Card[] deck_list = new Card[52];
            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                deck_list[i * 4] = new Card(CardValueExt.GetCardValueFromInt(i + 1), Suit.CLUBS);
                deck_list[i * 4 + 1] = new Card(CardValueExt.GetCardValueFromInt(i + 1), Suit.DIAMONDS);
                deck_list[i * 4 + 2] = new Card(CardValueExt.GetCardValueFromInt(i + 1), Suit.HEARTS);
                deck_list[i * 4 + 3] = new Card(CardValueExt.GetCardValueFromInt(i + 1), Suit.SPADES);
            }
            deck_list = deck_list.OrderBy(i => Guid.NewGuid()).ToArray();
            foreach (Card c in deck_list) {
                deck.Enqueue(c);
            }
            return deck;
        }

        public Card Deal1() {
            return deck.Dequeue();
        }

    }
}
