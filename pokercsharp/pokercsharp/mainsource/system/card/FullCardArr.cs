﻿using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.card {
    class FullCardArr {

        const int FULL_DECK_LEN = 52;
        public static Card[] card_arr;

        public FullCardArr() {
            card_arr = new Card[FULL_DECK_LEN];
            for (int i = 0; i < FULL_DECK_LEN; ++i) {
                Card c = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
                card_arr[c.GetHashCode()] = c;
            }
        }

        public Card[] GetCardArr() {
            return card_arr;
        }

    }
}