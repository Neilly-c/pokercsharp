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

        protected readonly int BarCount = 5;
        public double[] values { get; set; }
        public double prob { get; set; }
        public List<Node> nodes { get; set; } = new List<Node>();
        protected ColumnDefinition[] columns;
        protected RowDefinition[] rows;
        protected readonly SolidColorBrush[] pallet = new SolidColorBrush[5] { Brushes.Tomato, Brushes.Gold, Brushes.GreenYellow, Brushes.DodgerBlue, Brushes.DeepPink };
        protected TextBlock textBlock;

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
            textBlock = new TextBlock();
            Grid.SetColumnSpan(textBlock, BarCount);
            Grid.SetRowSpan(textBlock, 2);
            textBlock.FontSize = 16;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.Foreground = Brushes.White;
            this.Children.Add(textBlock);
            SetProb(1);
        }

        public void Render() {
            Array.Fill(values, 0);
            double valueSum = 0;
            foreach(Node n in nodes) {
                double[] strategys = n.GetAverageStrategy();
                for(int i = 0; i < strategys.Length; ++i) {
                    values[i] += strategys[i];
                    valueSum += strategys[i];
                }
            }
            if(valueSum == 0) {
                valueSum = 1;
            }
            for (int i = 0; i < BarCount; ++i) {
                if (i < values.Length) {
                    columns[i].Width = GridLengths.lengths[(int)(values[i] / valueSum * 16)];
                } else {
                    columns[i].Width = GridLengths.lengths[0];
                }
            }
            rows[1].Height = GridLengths.lengths[(int)(prob * 16)];
            rows[0].Height = GridLengths.lengths[(int)(prob * 16)];
        }

        public void AddNode(Node newNode) {
            nodes.Add(newNode);
        }

        public virtual void SetProb(double p) {
            if (p < 0) {
                p = 0;
            }else if (p > 1) {
                p = 1;
            }
            prob = p;
            int pInt = (int)(p * 16);
            rows[1].Height = GridLengths.lengths[pInt];
            rows[0].Height = GridLengths.lengths[16 - pInt];
        }

        public void SetValue(double[] values) {
            if (values.Length > BarCount) {
                Debug.WriteLine("too many values");     //not safe
            } else {
                this.values = values;
                double valueSum = 0;
                foreach(double d in values) {
                    valueSum += d;
                }
                if(valueSum == 0) {
                    valueSum = 1;
                    values[0] = 1;
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

        private int loop = 0;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args) {
            string str = "";
            foreach(Node n in nodes) {
                str += n.infoSet + "; ";
            }
            for (int i = 0; i < values.Length; ++i) {
                str += string.Format("{0:f4}", values[i]);
                str += ", ";
            }
            Console.WriteLine(str);
        }

        public void SetText(string str) {
            this.textBlock.Text = str;
        }

    }
}
