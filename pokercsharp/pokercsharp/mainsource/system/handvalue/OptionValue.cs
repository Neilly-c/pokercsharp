using mainsource.system.card;
using System;
using System.Collections.Generic;

namespace mainsource.system.handvalue {


    public class OptionValue : IComparable<OptionValue> {

        private readonly CardValue value1;
        private readonly CardValue? value2;
        private readonly CardValue? value3;
        private readonly CardValue? value4;
        private readonly CardValue? value5;
        private static Dictionary<HandName, int> reservedValues = new Dictionary<HandName, int>() {
            {HandName.ROYAL_FLUSH, 1 },
            {HandName.STRAIGHT_FLUSH, 1 },
            {HandName.QUADS, 2 },
            {HandName.FULL_HOUSE, 2 },
            {HandName.FLUSH, 5 },
            {HandName.STRAIGHT, 1 },
            {HandName.SET, 3 },
            {HandName.TWO_PAIRS, 3 },
            {HandName.ONE_PAIR, 4 },
            {HandName.HIGH_CARD, 5 }
        };

        internal OptionValue(CardValue value1) {
            this.value1 = value1;
        }

        internal OptionValue(CardValue value1, CardValue value2)
            : this(value1) {
            this.value2 = value2;
        }

        internal OptionValue(CardValue value1, CardValue value2, CardValue value3)
            : this(value1, value2) {
            this.value3 = value3;
        }

        internal OptionValue(CardValue value1, CardValue value2, CardValue value3, CardValue value4)
            : this(value1, value2, value3) {
            this.value4 = value4;
        }

        internal OptionValue(CardValue value1, CardValue value2, CardValue value3, CardValue value4, CardValue value5)
            : this(value1, value2, value3, value4) {
            this.value5 = value5;
        }

        public CardValue getValue1() {
            return value1;
        }

        public CardValue? getValue2() {
            return value2;
        }

        public CardValue? getValue3() {
            return value3;
        }

        public CardValue? getValue4() {
            return value4;
        }

        public CardValue? getValue5() {
            return value5;
        }

        public int GetValue(HandName handName) {
            CardValue?[] values = { value1, value2, value3, value4, value5 };
            int value = 0;
            for (int i = 0; i < reservedValues[handName]; ++i) {
                if (values[i] == null) {
                    break;
                }
                CardValue c = (CardValue)values[i];
                value += values[i].Equals(CardValue.ACE) ? 14 : c.GetValue();
                value *= 16;
            }
            return value;
        }
        
        public override int GetHash(){
            CardValue?[] values = { value1, value2, value3, value4, value5 };
            int hash = 0;
            for (int i = 0; i < 5; ++i) {
                if (values[i] != null) {
                    hash += values[i];
                }
                if(i < 4){
                    hash *= 14;
                }
            }
            return hash;
        }

        public String GetDetail(HandName handName) {
            switch (handName) {
                case HandName.ROYAL_FLUSH:
                    return "ROYAL FLUSH";
                case HandName.STRAIGHT_FLUSH:
                case HandName.STRAIGHT:
                    return getValue1().ToString() + " high";
                case HandName.QUADS:
                    return getValue1().ToString() + ", kicker " + getValue2().ToString();
                case HandName.FULL_HOUSE:
                    return getValue1().ToString() + " full of " + getValue2().ToString();
                case HandName.FLUSH:
                case HandName.HIGH_CARD:
                    return getValue1().ToString() + "high, " + getValue2().ToString() + ", " + getValue3() + ", " + getValue4() + ", " + getValue5();
                case HandName.SET:
                    return getValue1().ToString() + ", kicker " + getValue2().ToString() + ", " + getValue3().ToString();
                case HandName.TWO_PAIRS:
                    return getValue1().ToString() + ", " + getValue2().ToString() + ", kicker " + getValue3().ToString();
                case HandName.ONE_PAIR:
                    return getValue1().ToString() + ", kicker " + getValue2().ToString() + ", " + getValue3().ToString() + ", " + getValue4().ToString();
            }
            return getValue1().ToString();
        }

        public int CompareTo(OptionValue o) {
            if (!this.getValue1().Equals(o.getValue1())) {
                if (this.getValue1().Equals(CardValue.ACE))
                    return 1;
                if (o.getValue1().Equals(CardValue.ACE))
                    return -1;
                return this.getValue1().GetValue() - o.getValue1().GetValue();
            } else if (!this.getValue2().Equals(o.getValue2())) {
                if (this.getValue2().Equals(CardValue.ACE))
                    return 1;
                if (o.getValue2().Equals(CardValue.ACE))
                    return -1;
                return this.getValue2().GetValueOrDefault() - o.getValue2().GetValueOrDefault();
            } else if (!this.getValue3().Equals(o.getValue3())) {
                if (this.getValue3().Equals(CardValue.ACE))
                    return 1;
                if (o.getValue3().Equals(CardValue.ACE))
                    return -1;
                return this.getValue3().GetValueOrDefault() - o.getValue3().GetValueOrDefault();
            } else if (!this.getValue4().Equals(o.getValue4())) {
                if (this.getValue4().Equals(CardValue.ACE))
                    return 1;
                if (o.getValue4().Equals(CardValue.ACE))
                    return -1;
                return this.getValue4().GetValueOrDefault() - o.getValue4().GetValueOrDefault();
            } else if (!this.getValue5().Equals(o.getValue5())) {
                if (this.getValue5().Equals(CardValue.ACE))
                    return 1;
                if (o.getValue5().Equals(CardValue.ACE))
                    return -1;
                return this.getValue5().GetValueOrDefault() - o.getValue5().GetValueOrDefault();
            } else {
                return 0;
            }
        }
    }
}
