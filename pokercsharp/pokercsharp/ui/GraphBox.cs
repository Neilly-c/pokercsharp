using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pokercsharp.ui {
    class GraphBox : Grid {

        readonly int BarCount = 3;
        ColumnDefinition[] columns;
        private readonly SolidColorBrush[] pallet = new SolidColorBrush[3] { Brushes.Tomato, Brushes.GreenYellow, Brushes.DodgerBlue };

        public GraphBox(int BarCount) : this(){
            this.BarCount = BarCount;
        }

        public GraphBox() {
            columns = new ColumnDefinition[BarCount];
            for(int i = 0; i < BarCount; ++i) {
                columns[i] = new ColumnDefinition();
                columns[i].Width = GridLengths.lengths[1];
                this.ColumnDefinitions.Add(columns[i]);
                Border border = new Border();
                border.Background = pallet[i % pallet.Length];
                Grid.SetColumn(border, i);
                this.Children.Add(border);
            }
        }

        public void setValue(double[] values) {
            if (values.Length > BarCount) {
                Debug.WriteLine("too many values");
            } else {
                double valueSum = 0;
                foreach(double d in values) {
                    valueSum += d;
                }
                for(int i = 0; i < BarCount; ++i) {
                    if (i < values.Length) {
                        columns[i].Width = GridLengths.lengths[(int)(values[i] / valueSum * 16)];
                    } else {
                        columns[i].Width = GridLengths.lengths[0];
                    }
                }
            }
        }
    }
}
