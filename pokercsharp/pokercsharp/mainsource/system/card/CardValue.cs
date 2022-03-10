using System;

namespace mainsource.system.card {

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

        /// <summary>
        /// A,2,3,4,....,J,Q,K order
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int GetValue(this CardValue val) {
            return (int)val;
        }

        /// <summary>
        /// A,K,Q,J,....,3,2 order
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int GetPower(this CardValue val) {
            return (14 - GetValue(val)) % 13;
        }

        public static string GetAbb(this CardValue val) {
            string[] abb = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K" };
            return abb[(int)val - 1];
        }

        public static CardValue GetCardValueFromInt(int i) {
            foreach (CardValue c in Enum.GetValues(typeof(CardValue))) {
                if (c.GetValue() == i)
                    return c;
            }
            return CardValue.ACE;
        }
    }
}