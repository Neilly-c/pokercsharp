﻿using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.card {
    class FullCardArr {

        public Card[] card_arr { get; set; }

        public string[] hand_abbreviated_arr { get; set; }

        public FullCardArr() {
            card_arr = new Card[Constants.FULL_DECK_LEN];
            for (int i = 0; i < Constants.FULL_DECK_LEN; ++i) {
                Card c = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
                card_arr[c.GetHashCode()] = c;
            }
            List<string> hand_abb_list = new List<string>();
            string[] template_str = { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };
            for (int i = 0; i < template_str.Length; ++i) {
                hand_abb_list.Add(template_str[i] + template_str[i]);
                hand_abb_list.Add(template_str[i] + template_str[i]);
                hand_abb_list.Add(template_str[i] + template_str[i]);
                for (int j = i + 1; j < template_str.Length; ++j) {
                    hand_abb_list.Add(template_str[i] + template_str[j] + "s");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "s");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                    hand_abb_list.Add(template_str[i] + template_str[j] + "o");
                }
            }
            hand_abbreviated_arr = hand_abb_list.ToArray();
        }

    }
}
