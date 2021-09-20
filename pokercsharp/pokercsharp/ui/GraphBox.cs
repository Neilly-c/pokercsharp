using pokercsharp.mainsource.cfrplus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace pokercsharp.ui {
    class GraphBox : Grid {

        readonly int BarCount = 5;
        public double[] values { get; set; }
        public double ps { get; set; }
        public List<Node> nodes { get; set; } = new List<Node>();
        ColumnDefinition[] columns;
        RowDefinition[] rows;
        private readonly SolidColorBrush[] pallet = new SolidColorBrush[5] { Brushes.Tomato, Brushes.Gold, Brushes.GreenYellow, Brushes.DodgerBlue, Brushes.DeepPink };

        public GraphBox(int BarCount) : this(){
            this.BarCount = BarCount;
        }

        public GraphBox() {
            values = new double[BarCount];
            rows = new RowDefinition[2];
            rows[0] = new RowDefinition();
            rows[1] = new RowDefinition();
            rows[0].Height = GridLengths.lengths[1];
            rows[1].Height = GridLengths.lengths[1];
            this.RowDefinitions.Add(rows[0]);
            this.RowDefinitions.Add(rows[1]);

            Border blank = new Border();
            blank.Background = Brushes.Gray;
            Grid.SetColumnSpan(blank, BarCount);
            Grid.SetRow(blank, 0);
            this.Children.Add(blank);

            columns = new ColumnDefinition[BarCount];
            for (int i = 0; i < BarCount; ++i) {
                columns[i] = new ColumnDefinition();
                columns[i].Width = GridLengths.lengths[1];
                this.ColumnDefinitions.Add(columns[i]);
                Border border = new Border();
                border.Background = pallet[i % pallet.Length];
                Grid.SetColumn(border, i);
                Grid.SetRow(border, 1);
                this.Children.Add(border);
            }
        }

        public void AddNode(Node newNode) {
            nodes.Add(newNode);
            Array.Fill(values, 0);
            ps = 0;
            double valueSum = 0;
            foreach(Node n in nodes) {
                double[] strategys = n.GetAverageStrategy();
                for(int i = 0; i < strategys.Length; ++i) {
                    values[i] += strategys[i];
                    valueSum += strategys[i];
                }
                ps += n.plr == 0 ? n.p0 : n.p1;
            }
            for (int i = 0; i < BarCount; ++i) {
                if (i < values.Length) {
                    columns[i].Width = GridLengths.lengths[(int)(values[i] / valueSum * 16)];
                } else {
                    columns[i].Width = GridLengths.lengths[0];
                }
            }
            rows[1].Height = GridLengths.lengths[Math.Min((int)(ps * 16), 15)];
            rows[0].Height = GridLengths.lengths[Math.Max(15 - (int)(ps * 16), 0)];
        }

        public void SetValue(double[] values) {
            if (values.Length > BarCount) {
                Debug.WriteLine("too many values");
            } else {
                this.values = values;
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args) {
            string str = "";
            for(int i = 0; i < values.Length; ++i) {
                str += string.Format("{0:f4}", values[i]);
                str += ", ";
            }
            str += "\n";
            foreach (Node n in nodes) {
                str += n.infoSet + ": " + n.p0 + ", " + n.p1 + ", " + n.p_ + ", p" + n.plr;
            }
            Console.WriteLine(str);
        }

    }
}
