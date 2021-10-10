using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace pokercsharp.ui {
    class InputGraphBox : GraphBox {

        public InputGraphBox() : base() {
            base.rows[0].Height = GridLengths.lengths[0];
        }

        private int loop = 0;
        public string[] actions { get; set; } = new string[3] { "Fold", "Call", "Raise" };

        public void SetActions(string[] actions) {
            this.actions = actions;
        }

        public override void SetProb(double p) {
            base.SetProb(p);
        }
    }
}
