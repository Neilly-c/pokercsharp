namespace mainsource.system.handvalue {


    public class OptionValue : Comparable<OptionValue> {

        private sealed CardValue value1;
        @Nullable private sealed CardValue value2;
        @Nullable private sealed CardValue value3;
        @Nullable private sealed CardValue value4;
        @Nullable private sealed CardValue value5;
        private static sealed Map<HandName, Integer> reservedValues = new HashMap<>();
        static {
        reservedValues.put(HandName.ROYAL_FLUSH, 1);
        reservedValues.put(HandName.STRAIGHT_FLUSH, 1);
        reservedValues.put(HandName.QUADS, 2);
        reservedValues.put(HandName.FULL_HOUSE, 2);
        reservedValues.put(HandName.FLUSH, 5);
        reservedValues.put(HandName.STRAIGHT, 1);
        reservedValues.put(HandName.SET, 3);
        reservedValues.put(HandName.TWO_PAIRS, 3);
        reservedValues.put(HandName.ONE_PAIR, 4);
        reservedValues.put(HandName.HIGH_CARD, 5);
    }

    public OptionValue(CardValue value1) {
        this(value1, null);
    }

    public OptionValue(CardValue value1, CardValue value2) {
        this(value1, value2, null);
    }

    public OptionValue(CardValue value1, CardValue value2, CardValue value3) {
        this(value1, value2, value3, null);
    }

    public OptionValue(CardValue value1, CardValue value2, CardValue value3, CardValue value4) {
        this(value1, value2, value3, value4, null);
    }

    public OptionValue(CardValue value1, CardValue value2, CardValue value3, CardValue value4, CardValue value5) {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
        this.value5 = value5;
    }

    public CardValue getValue1() {
        return value1;
    }

    public CardValue getValue2() {
        return value2;
    }

    public CardValue getValue3() {
        return value3;
    }

    public CardValue getValue4() {
        return value4;
    }

    public CardValue getValue5() {
        return value5;
    }

    public int getValue(HandName handName) {
        CardValue[] values = { value1, value2, value3, value4, value5 };
        int value = 0;
        for (int i = 0; i < reservedValues.get(handName); ++i) {
            if (values[i] == null) {
                break;
            }
            value += values[i].equals(CardValue.ACE) ? 14 : values[i].getValue();
            value *= 16;
        }
        return value;
    }

    public String getDetail(HandName handName) {
        switch (handName) {
            case ROYAL_FLUSH:
                return "ROYAL FLUSH";
            case STRAIGHT_FLUSH:
            case STRAIGHT:
                return getValue1().toString() + " high";
            case QUADS:
                return getValue1().toString() + ", kicker " + getValue2().toString();
            case FULL_HOUSE:
                return getValue1().toString() + " full of " + getValue2().toString();
            case FLUSH:
            case HIGH_CARD:
                return getValue1().toString() + "high, " + getValue2().toString() + ", " + getValue3() + ", " + getValue4() + ", " + getValue5();
            case SET:
                return getValue1().toString() + ", kicker " + getValue2().toString() + ", " + getValue3().toString();
            case TWO_PAIRS:
                return getValue1().toString() + ", " + getValue2().toString() + ", kicker " + getValue3().toString();
            case ONE_PAIR:
                return getValue1().toString() + ", kicker " + getValue2().toString() + ", " + getValue3().toString() + ", " + getValue4().toString();
        }
        return getValue1().toString();
    }

    public override int compareTo(OptionValue o) {
        if (!this.getValue1().equals(o.getValue1())) {
            if (this.getValue1().equals(CardValue.ACE))
                return 1;
            if (o.getValue1().equals(CardValue.ACE))
                return -1;
            return this.getValue1().getValue() - o.getValue1().getValue();
        } else if (!this.getValue2().equals(o.getValue2())) {
            if (this.getValue2().equals(CardValue.ACE))
                return 1;
            if (o.getValue2().equals(CardValue.ACE))
                return -1;
            return this.getValue2().getValue() - o.getValue2().getValue();
        } else if (!this.getValue3().equals(o.getValue3())) {
            if (this.getValue3().equals(CardValue.ACE))
                return 1;
            if (o.getValue3().equals(CardValue.ACE))
                return -1;
            return this.getValue3().getValue() - o.getValue3().getValue();
        } else if (!this.getValue4().equals(o.getValue4())) {
            if (this.getValue4().equals(CardValue.ACE))
                return 1;
            if (o.getValue4().equals(CardValue.ACE))
                return -1;
            return this.getValue4().getValue() - o.getValue4().getValue();
        } else if (!this.getValue5().equals(o.getValue5())) {
            if (this.getValue5().equals(CardValue.ACE))
                return 1;
            if (o.getValue5().equals(CardValue.ACE))
                return -1;
            return this.getValue5().getValue() - o.getValue5().getValue();
        } else {
            return 0;
        }
    }

}
