using System;

namespace mainsource.system.game {

    public class NewChips {

        protected int[] chipCount;
        protected int[] betCount;
        protected int plrs;
        protected int pot = 0;

        public NewChips() : this(2, 500) {

        }

        public NewChips(int plrs, int chip_count) {
            if (plrs < 2 || plrs > 10) {
                throw new TableException("players must be between 2 to 10");
            }
            if (chip_count < 0) {
                throw new TableException("chip count must be positive");
            }
            chipCount = new int[plrs];
            betCount = new int[plrs];
            this.plrs = plrs;
            Array.Fill(chipCount, chip_count);
        }

        public void Raise(int p, int val) {
            if (p < 0 || p >= plrs) {
                throw new TableException("invalid number of players");
            }
            if (chipCount[p] < val - betCount[p]) {
                throw new TableException("invalid bet");
            }
            chipCount[p] -= (val - betCount[p]);
            betCount[p] = val;
        }

        public void Call(int p) {
            if (p < 0 || p >= plrs) {
                throw new TableException("invalid number of players");
            }
            int bet_max = 0;
            for (int i = 0; i < plrs; ++i) {
                if (betCount[i] > bet_max) {
                    bet_max = betCount[i];
                }
            }
            if (bet_max == betCount[p]) {
                throw new TableException("invalid call");
            }
            chipCount[p] -= (bet_max - betCount[p]);
            betCount[p] = bet_max;
        }

        public void Resume() {
            foreach (int i in betCount) {
                pot += i;
            }
            Array.Fill(betCount, 0);
        }

        public void WinsPot(int p) {
            chipCount[p] += pot;
            pot = 0;
        }

        public void SplitPot(params int[] p) {
            int eachPot = pot / p.Length;
            for (int i = 0; i < p.Length; i++) {
                chipCount[p[i]] += eachPot;
                if (pot % eachPot < i) {
                    ++chipCount[p[i]];
                }
            }
            pot = 0;
        }

    }
}