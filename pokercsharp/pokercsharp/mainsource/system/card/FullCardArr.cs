using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system.card {
    class FullCardArr {

        /// <summary>
        /// 52枚のカードの配列
        /// </summary>
        public Card[] card_arr { get; set; }

        /// <summary>
        /// 52枚のカード(string)の配列
        /// </summary>
        public string[] card_str_arr { get; set; }

        /// <summary>
        /// ハンドの組み合わせ(略記)の配列 
        /// スーテッド:ペア:オフスート=2:3:6の割合で入っている
        /// </summary>
        public string[] hand_abbreviated_arr { get; set; }

        public FullCardArr() {
            card_arr = new Card[Constants.FULL_DECK_LEN];
            for (int i = 0; i < Constants.FULL_DECK_LEN; ++i) {
                Card c = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));
                card_arr[c.GetHashCode()] = c;
            }
            List<string> card_str_list = new List<string>();
            foreach(Card c in card_arr) {
                card_str_list.Add(c.ToAbbreviateString());
            }
            card_str_arr = card_str_list.ToArray();
            List<string> hand_abb_list = new List<string>();
            string[] template_str = { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };
            for (int i = 0; i < template_str.Length; ++i) {
                hand_abb_list.Add(template_str[i] + template_str[i] + "_");
                hand_abb_list.Add(template_str[i] + template_str[i] + "_");
                hand_abb_list.Add(template_str[i] + template_str[i] + "_");
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
