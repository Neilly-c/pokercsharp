namespace mainsource.system.card;

public enum Suit {

    CLUBS = 1,
    DIAMONDS,
    HEARTS,
    SPADES
}

public class SuitExt{
  
    public int getValue(this Suit suit) {
        return (int)suit;
    }

    public string getAbb(this Suit suit) {
      string[] abb = {"c", "d", "h", "s"};
      return abb[(int)suit];
    }
}