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

        protected override void OnDragEnter(DragEventArgs e) {
            this.OnMouseLeftButtonDown(null);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args) {
            double[] decision = new double[3];
            decision[loop % 3] = 100;
            ++loop;
            SetValue(decision);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            base.OnMouseRightButtonDown(e);
        }

        public override void SetProb(double p) {
            base.SetProb(p);
        }
    }
}
