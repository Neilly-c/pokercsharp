using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.card {
    class FullCardArr {

        public static Card[] card_arr;

        public FullCardArr() {
            card_arr = new Card[Constants.FULL_DECK_LEN];
            for (int i = 0; i < Constants.FULL_DECK_LEN; ++i) {
                Card c = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
                card_arr[c.GetHashCode()] = c;
            }
        }

        public Card[] GetCardArr() {
            return card_arr;
        }

    }
}
