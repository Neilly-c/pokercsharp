namespace mainsource.system.card
{

    public enum Suit
    {

        CLUBS = 1,
        DIAMONDS,
        HEARTS,
        SPADES
    }

    public static class SuitExt
    {

        public static int GetValue(this Suit suit)
        {
            return (int)suit;
        }

        public static string GetAbb(this Suit suit)
        {
            string[] abb = { "c", "d", "h", "s" };
            return abb[(int)suit];
        }
    }
}