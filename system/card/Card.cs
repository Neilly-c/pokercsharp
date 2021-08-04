namespace mainsource.system.card;

public sealed class Card : Comparable<Card> {

    private sealed CardValue value;
    private sealed Suit suit;

    public Card(CardValue value, Suit suit) {
        this.value = value;
        this.suit = suit;
    }

    public CardValue getValue() {
        return value;
    }

    public Suit getSuit() {
        return suit;
    }

    public int getNumber() {
        return this.value.getValue() + 13 * this.suit.getValue() - 14;
    }

    public override String toString() {
        return this.value.name() + " of " + this.suit.name() + ": " + this.getNumber();
    }

    public String toAbbreviateString(){
        return this.value.getAbb() + this.suit.getAbb();
    }

    public override bool equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }

        Card card = (Card) o;

        return getValue() == card.getValue() && getSuit() == card.getSuit();
    }

    public override int hashCode() {
        int result = getValue().hashCode();
        result = 31 * result + getSuit().hashCode();
        return result;
    }

    public override int compareTo(Card o) {
        sealed bool otherIsAnAce = CardValue.ACE.equals(o.getValue());
        sealed bool iamAnAce = CardValue.ACE.equals(this.value);
        if (iamAnAce && !otherIsAnAce) {
            return 1;
        }
        if (!iamAnAce && otherIsAnAce) {
            return -1;
        }
        return this.value.compareTo(o.getValue());
    }
}