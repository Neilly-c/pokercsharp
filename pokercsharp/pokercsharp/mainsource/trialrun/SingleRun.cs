using mainsource.system.card;
using mainsource.system.game;
using pokercsharp.mainsource.system.range;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pokercsharp.mainsource.trialrun {
    class SingleRun {

        private NewDeck newDeck;

        public SingleRun() {
            newDeck = new NewDeck();
        }

        /// <summary>
        /// 与えた戦略をもとに1ハンドプレイする
        /// </summary>
        /// <param name="strategys">戦略リスト(人数分)</param>
        /// <param name="plrs">プレイヤー数</param>
        /// <param name="stack">(ブラインド、アンティを払う前の)スタック</param>
        /// <param name="postSB">SB額,通常0.5d</param>
        /// <param name="postBB">BB額,通常1d</param>
        /// <param name="postAnte">アンティ,通常0</param>
        /// <param name="BTN">BTNのプレイヤーの位置 2人の場合はSBの位置 何も入れなければ0</param>
        /// <returns>各プレイヤーの利得</returns>
        public double[] RunSinglePushOrFold(Strategys[] strategys, int plrs, double stack, double postSB, double postBB, double postAnte, int? BTN) {/*
            newDeck.ReShuffle();
            Card[][] hole_cards = new Card[plrs][];
            String action_history = "";
            for (int i=0;i<plrs;++i) {              //カードを配りつつアクションに応じて戦略を引っ張ってきて実行
                hole_cards[i] = new Card[2];
                hole_cards[i][0] = newDeck.Deal1();
                hole_cards[i][1] = newDeck.Deal1();

                int a = (14 - hole_cards[i][0].GetValue().GetValue()) % 13;
                int b = (14 - hole_cards[i][1].GetValue().GetValue()) % 13;
                if(a > b) {
                    Swap(ref a, ref b);
                }
                if (!hole_cards[i][0].GetSuit().Equals(hole_cards[i][1].GetSuit())) {
                    Swap(ref a, ref b);
                }

                RangeStrategy strategy = strategys[i].strategyByActionFacing[action_history];
                double[] str = strategy.GetStrategyDict(a, b);
            }
            */
            return null;
        }

        static void Swap<T>(ref T m, ref T n) {
            T work = m;
            m = n;
            n = work;
        }

    }
}
