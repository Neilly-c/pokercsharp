using System;

namespace mainsource.system.card {

    public class Card : IComparable<Card> {

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

        public override string ToString() {
            return this.value.ToString() + " of " + this.suit.ToString() + ": " + this.GetHashCode();
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

        public override int GetHashCode() {
            return 13 * this.suit.GetValue() - this.value.GetValue();
        }

        public int CompareTo(Card o) {      //As,Ah,...,2d,2cの順
            bool otherIsAnAce = CardValue.ACE.Equals(o.GetValue());
            bool iamAnAce = CardValue.ACE.Equals(this.value);
            if (iamAnAce && !otherIsAnAce) {
                return 1;
            }
            if (!iamAnAce && otherIsAnAce) {
                return -1;
            }
            if (this.value.CompareTo(o.GetValue()) != 0) {
                return this.value.CompareTo(o.GetValue());
            }
            return this.suit.CompareTo(o.GetSuit());
        }
    }
}
