using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource {
    static class Constants {
        public static readonly int COMBINATION = 1326;
        public static readonly int FULL_GRID_SIZE = 1327;
        public static readonly int ABB_COM = 169;
        public static readonly int CARDVALUE_LEN = 13;
        public static readonly int FULL_DECK_LEN = 52;
        public static readonly int HAND_CARDS = 5;
        public static readonly int HOLDEM_HAND_CARDS = 7;
        public static readonly int COM_2 = 878485;
        public static readonly int _48C5 = 1712304;
        public static readonly int _52C5 = 2598960;
        public static readonly Dictionary<char, int> handKey = new Dictionary<char, int>() {
            { 'A', 0 },
            { 'K', 1 },
            { 'Q', 2 },
            { 'J', 3 },
            { 'T', 4 },
            { '9', 5 },
            { '8', 6 },
            { '7', 7 },
            { '6', 8 },
            { '5', 9 },
            { '4', 10 },
            { '3', 11 },
            { '2', 12 }
        };
    }
}
