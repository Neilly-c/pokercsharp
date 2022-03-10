using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.card {
    class CardToHandConverter {
        public static string ConvertCardsToHandStr(Card[] card) {
            int a = card[0].GetValue().GetPower(),
                b = card[1].GetValue().GetPower();
            if(a == b) {
                return card[0].GetValue().GetAbb() + card[1].GetValue().GetAbb() + "_";
            }else if(a < b) {
                return card[0].GetValue().GetAbb() + card[1].GetValue().GetAbb() + (card[0].GetSuit().Equals(card[1].GetSuit()) ? "s" : "o");
            } else {
                return card[1].GetValue().GetAbb() + card[0].GetValue().GetAbb() + (card[0].GetSuit().Equals(card[1].GetSuit()) ? "s" : "o");
            }
        }

        public static string ConvertCardsToHandStr(Card card1, Card card2) {
            int a = card1.GetValue().GetPower(),
                b = card2.GetValue().GetPower();
            if (a == b) {
                return card1.GetValue().GetAbb() + card2.GetValue().GetAbb() + "_";
            } else if (a < b) {
                return card1.GetValue().GetAbb() + card2.GetValue().GetAbb() + (card2.GetSuit().Equals(card1.GetSuit()) ? "s" : "o");
            } else {
                return card2.GetValue().GetAbb() + card1.GetValue().GetAbb() + (card1.GetSuit().Equals(card2.GetSuit()) ? "s" : "o");
            }
        }
    }
}
