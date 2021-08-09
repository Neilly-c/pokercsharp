namespace mainsource.system.game {

    public class NewDeck {

        const int FULL_DECK_LEN = 52;
        const int CARDVALUE_LEN = 13;
        private Queue<Card> deck = new Queue<Card>();

        public NewDeck() {
            initdeck();
        }

        private Queue<Card> initdeck() {
            deck = new Queue<Card>();
            for (int i = 0; i < CARDVALUE_LEN; ++i) {
                deck.Add(new Card(getCardValueFromInt(i + 1), Suit.CLUBS));
                deck.Add(new Card(getCardValueFromInt(i + 1), Suit.DIAMONDS));
                deck.Add(new Card(getCardValueFromInt(i + 1), Suit.HEARTS));
                deck.Add(new Card(getCardValueFromInt(i + 1), Suit.SPADES));
            }
            deck = deck.OrderBy(a => Guid.NewGuid()).ToList();
            return deck;
        }

        public Card deal1() {
            return deck.poll();
        }

    }
}
