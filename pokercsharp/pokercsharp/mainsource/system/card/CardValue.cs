using System;

namespace mainsource.system.card
{

    public enum CardValue
    {

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

    public static class CardValueExt
    {


        public static int GetValue(this CardValue val)
        {
            return (int)val;
        }

        public static string GetAbb(this CardValue val)
        {
            string[] abb = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K" };
            return abb[(int)val];
        }

        public static CardValue GetCardValueFromInt(int i)
        {
            foreach (CardValue c in Enum.GetValues(typeof(CardValue)))
            {
                if (c.GetValue() == i)
                    return c;
            }
            return CardValue.ACE;
        }
    }
}