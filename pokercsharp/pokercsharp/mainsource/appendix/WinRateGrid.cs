using mainsource.system.card;
using mainsource.system.evaluator;
using mainsource.system.handvalue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace pokercsharp.mainsource.appendix {
    class WinRateGrid {
        const int COMBINATION = 1326;
        const int FULL_DECK_LEN = 52;

        public void Init() {
            int[][] full_grid = new int[COMBINATION][];     //1326*1326
            Card[] card_arr = new Card[FULL_DECK_LEN];      //ただの52枚のカードの配列
            Card[][] hand_arr = new Card[COMBINATION][];    //1326通りのハンドの配列
            for (int i = 0; i < COMBINATION; ++i) {
                full_grid[i] = new int[COMBINATION];
                hand_arr[i] = new Card[2];                  //配列初期化処理(javaと違って2次元配列はこうやって定義しないといけないらしい)
            }
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                card_arr[i] = new Card(CardValueExt.GetCardValueFromInt(1 + (i / 4)), SuitExt.GetSuitFromInt(1 + (i % 4)));     //カード配列生成
            }

            int iter = 0;
            for(int i = 0; i < FULL_DECK_LEN; ++i) {
                for (int j = i + 1; j < FULL_DECK_LEN; ++j) {       //ハンド配列生成
                    hand_arr[iter][0] = card_arr[i];
                    hand_arr[iter][1] = card_arr[j];
                    ++iter;
                }
            }

            int loop = 0
            HoldemHandEvaluator evaluator = new HoldemHandEvaluator();  //ハンド評価クラス　当たり前だけどこれが軽くなればだいぶ変わる

            for(int i = 0; i < COMBINATION; ++i) {
                for(int j = i + 1; j < COMBINATION; ++j) {      // 1326 * 1325 / 2 = 878475 loops.
                    
                    if(full_grid[i][j] != 0){
                        continue;
                    }

                    int p1_0 = 0, p1_1 = 0, p2_0 = 0, p2_1 = 0;
                    for(int k = 0; k < FULL_DECK_LEN; ++k) {
                        if (card_arr[k].Equals(hand_arr[i][0])) {
                            p1_0 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[i][1])) {
                            p1_1 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[j][0])) {
                            p2_0 = k;
                        }
                        if (card_arr[k].Equals(hand_arr[j][1])) {
                            p2_1 = k;
                        }
                    }
                    if(!IsAllDifferent(p1_0, p1_1, p2_0, p2_1) ){   //持ってるカードが被ってるハンドは除外
                        full_grid[i][j] = -1;
                        full_grid[j][i] = -1;
                        continue;
                    }
                    List<Card> card_list_edit = new List<Card>(card_arr);   //すでに使っているカードをデッキから除く
                    card_list_edit.Remove(hand_arr[i][0]);
                    card_list_edit.Remove(hand_arr[i][1]);
                    card_list_edit.Remove(hand_arr[j][0]);
                    card_list_edit.Remove(hand_arr[j][1]);
                    
                    if(card_list_edit.Length != 48){
                        Debug.WriteLine("HALT!!!");
                    }
                    
                    int winCount = 0, count = 0;        //winCount 勝ったハンドは+2，引き分けたハンドは+1する　countは常時+2する
                    Stopwatch sw = new Stopwatch();     //処理時間計測用
                    sw.Start();
                    for(int s = 0; s < card_list_edit.Length; ++s) {            //52_C_5 = 2598960 -> 48_C_5 = 1712304 loops.
                        for(int t = s + 1; t < card_list_edit.Length; ++t) {
                            for (int u = t + 1; u < card_list_edit.Length; ++u) {
                                for (int v = u + 1; v < card_list_edit.Length; ++v) {
                                    for (int w = v + 1; w < card_list_edit.Length; ++w) {       //これなんとかならんの？？？
                                        Card[] board = new Card[] { card_list_edit[s], card_list_edit[t], card_list_edit[u], card_list_edit[v], card_list_edit[w] };
                                        FinalHand 
                                            f_p1 = evaluator.Evaluate(hand_arr[i], board),  //ハンドとボードの情報を渡して完成ハンドを返してもらう
                                            f_p2 = evaluator.Evaluate(hand_arr[j], board);
                                        if(f_p1.CompareTo(f_p2) > 0) {  //クラスFinalHandはIComparableを継承しているのでCompareTo()で比較できる 比較大なら勝ち
                                            winCount += 2;
                                        }else if(f_p1.CompareTo(f_p2) == 0) {   //イコールなら引き分け
                                            winCount += 1;
                                        }
                                        count += 2;
                                        if(count % 20000 == 19998) {
                                            sw.Stop();
                                            Debug.WriteLine($"　{sw.ElapsedMilliseconds}ミリ秒");
                                            sw.Start();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    full_grid[i][j] = winCount;         //集計して勝った回数を書いておく　実際に使う時は分母で割って確率を出せばよい(double型より軽く正確)
                    full_grid[j][i] = count - winCount; //逆のマッチアップは勝率も逆にしておく

                    ++loop;
                    Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + "-"
                        + hand_arr[j][0].ToAbbreviateString() + hand_arr[j][1].ToAbbreviateString() + ", " + 
                        loop + " complete, " + full_grid[i][j] + "-" + full_grid[j][i]);
                    
                    for(int i_ = i; i_ < COMBINATION; ++i_) {       //計算省略できるところを省略するために調査する
                        for(int j_ = i_ + 1; j_ < COMBINATION; ++j_) {      // MAX 1326 * 1325 / 2 = 878475 loops.
                            if(full_grid[i_][j_] != 0){     //もう埋まってるところはパス
                                continue;
                            }
                            CardValue 
                                cv_i0 = hand_arr[i][0].GetCardValue(),
                                cv_i1 = hand_arr[i][1].GetCardValue(),
                                cv_j0 = hand_arr[j][0].GetCardValue(),
                                cv_j1 = hand_arr[j][1].GetCardValue(),
                                cv_i_0 = hand_arr[i_][0].GetCardValue(),
                                cv_i_1 = hand_arr[i_][1].GetCardValue(),
                                cv_j_0 = hand_arr[j_][0].GetCardValue(),
                                cv_j_1 = hand_arr[j_][1].GetCardValue();
                            Suit 
                                s_i0 = hand_arr[i][0].GetSuit(),
                                s_i1 = hand_arr[i][1].GetSuit(),
                                s_j0 = hand_arr[j][0].GetSuit(),
                                s_j1 = hand_arr[j][1].GetSuit(),
                                s_i_0 = hand_arr[i_][0].GetSuit(),
                                s_i_1 = hand_arr[i_][1].GetSuit(),
                                s_j_0 = hand_arr[j_][0].GetSuit(),
                                s_j_1 = hand_arr[j_][1].GetSuit();
                            if(!cv_i0.Equals(cv_i_0) || !cv_i1.Equals(cv_i_1) || !cv_j0.Equals(cv_j_0) || !cv_j1.Equals(cv_j_1)){
                                continue;   //数字が違うやつは結果が違うのでパス
                            }
                            if(s_i0.Equals(s_i1) == s_i_0.Equals(s_i_1)
                              && s_i0.Equals(s_j0) == s_i_0.Equals(s_j_0)
                              && s_i0.Equals(s_j1) == s_i_0.Equals(s_j_1)
                              && s_i1.Equals(s_j0) == s_i_1.Equals(s_j_0)
                              && s_i1.Equals(s_j1) == s_i_1.Equals(s_j_1)
                              && s_j0.Equals(s_j1) == s_j_0.Equals(s_j_1)){     //6通り全部の組み合わせが同じなら同じと見なす
                                full_grid[i_][j_] = winCount;
                                full_grid[j_][i_] = count - winCount;
                                Debug.Write(hand_arr[i_][0].ToAbbreviateString() + hand_arr[i_][1].ToAbbreviateString() + "-"
                                    + hand_arr[j_][0].ToAbbreviateString() + hand_arr[j_][1].ToAbbreviateString() + ", " + 
                                    " same as the former's, " + full_grid[i_][j_] + "-" + full_grid[j_][i_]);
                            }
                        }
                    }
                    
                }
            }

            for(int i = 0; i < COMBINATION; ++i) {
                Debug.Write(hand_arr[i][0].ToAbbreviateString() + hand_arr[i][1].ToAbbreviateString() + ",");
                if (i % 50 == 49) {
                    Debug.WriteLine("");
                }
            }

            for (int i = 0; i < COMBINATION; ++i) {
                Debug.WriteLine(string.Join(", ", full_grid[i]));
            }

        }

        private bool IsAllDifferent(params int[] ints) {
            for(int i = 0; i < ints.Length; ++i) {
                for(int j = i + 1; j < ints.Length; ++j) {
                    if(ints[i] == ints[j]) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
