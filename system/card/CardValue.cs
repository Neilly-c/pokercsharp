namespace mainsource.system.card;

public enum CardValue {

    ACE = 1,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING
}

public static class CardValueExt {
  

    public int getValue(this CardValue val) {
        return (int)val;
    }

    public string getAbb(this CardValue val) {
      string[] abb = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K"};
      return abb[(int)val];
    }

    public static CardValue getCardValueFromInt(int i){
        foreach(c in CardValue.GetValues())){
            if(c.getValue() == i)
                return c;
        }
        return ACE;
    }
}