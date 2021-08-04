namespace mainsource.system.handvalue;

public enum HandName {

    HIGH_CARD,
    ONE_PAIR,
    TWO_PAIRS,
    SET,
    STRAIGHT,
    FLUSH,
    FULL_HOUSE,
    QUADS,
    STRAIGHT_FLUSH,
    ROYAL_FLUSH
}

public class HandNameExt{

    public int getValue(this HandName handName) {
        return (int)handName;
    }
}