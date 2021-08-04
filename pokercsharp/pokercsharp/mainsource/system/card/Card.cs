using System;

namespace mainsource.system.card {

    public sealed class Card {

        private readonly CardValue value;
        private readonly Suit suit;

        public Card(CardValue value, Suit suit) {
            this.value = value;
            this.suit = suit;
        }

        public CardValue GetValue() {
            return value;
        }

        public Suit GetSuit() {
            return suit;
        }

        public int GetNumber() {
            return this.value.GetValue() + 13 * this.suit.GetValue() - 14;
        }

        public string ToString() {
            return this.value.ToString() + " of " + this.suit.ToString() + ": " + this.GetNumber();
        }

        public string ToAbbreviateString(){
            return this.value.GetAbb() + this.suit.GetAbb();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || this.GetType() != o.GetType()) {
                return false;
            }

            Card card = (Card) o;

            return GetValue() == card.GetValue() && GetSuit() == card.GetSuit();
        }

        public int HashCode() {
            int result = (int)GetValue();
            result = 31 * result + (int)GetSuit();
            return result;
        }

        public int CompareTo(Card o) {
            bool otherIsAnAce = CardValue.ACE.Equals(o.GetValue());
            bool iamAnAce = CardValue.ACE.Equals(this.value);
            if (iamAnAce && !otherIsAnAce) {
                return 1;
            }
            if (!iamAnAce && otherIsAnAce) {
                return -1;
            }
            return this.value.CompareTo(o.GetValue());
        }
    }
}