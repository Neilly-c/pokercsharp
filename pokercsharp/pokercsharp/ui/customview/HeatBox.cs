using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace pokercsharp.ui {
    class HeatBox : Grid {

        readonly int TextRows = 2;
        RowDefinition[] rows;
        TextBlock[] blocks;
        Border bg;
        private double max = 255, min = 0, val;
        readonly int link = -1;

        public HeatBox(int link) : this() {
            this.link = link;
        }

        public HeatBox() {
            rows = new RowDefinition[TextRows];
            blocks = new TextBlock[TextRows];
            for (int i = 0; i < TextRows; ++i) {
                rows[i] = new RowDefinition();
                rows[i].Height = GridLengths.lengths[1];
                this.RowDefinitions.Add(rows[i]);
            }

            bg = new Border();
            bg.Background = Brushes.Gray;
            Grid.SetRowSpan(bg, TextRows);
            this.Children.Add(bg);

            for (int i = 0; i < TextRows; ++i) {
                blocks[i] = new TextBlock();
                Grid.SetRow(blocks[i], i);
                this.Children.Add(blocks[i]);
                blocks[i].FontSize = 16;
                blocks[i].VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                blocks[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                blocks[i].TextAlignment = System.Windows.TextAlignment.Center;
            }
        }

        public void SetText(int row, string str) {
            switch (row) {
                case 0:
                    blocks[row].Text = FormatPercent(Int32.Parse(str));
                    break;
                default:
                    blocks[row].Text = str;
                    break;
            }
            if (link == row) {
                Render(Int32.Parse(str));
            }
        }

        public void SetFontSize(int row, int size) {
            blocks[row].FontSize = size;
        }

        public void Render(int val) {
            int balance = (int)((val - min) / (max - min) * 255);
            if (balance < 0) {
                balance = 0;
            } else if (balance >= 255) {
                balance = 255;
            }
            bg.Background = new SolidColorBrush(Color.FromArgb(0xFF, (byte)(0xFF - balance), (byte)(0xFF - balance / 2), (byte)balance));
        }

        public void Render(int val, int max) {
            SetMax(max);
            Render(val);
        }

        public void Render(int val, int max, int min) {
            SetMin(min);
            Render(val, max);
        }

        public void SetMax(int max) {
            this.max = max;
            if (max <= min) {
                Debug.WriteLine("param max must be bigger than param min. reset params, max = 255 & min = 0.");
                this.max = 255;
                this.min = 0;
            }
        }

        public void SetMin(int min) {
            this.min = min;
            if (max <= min) {
                Debug.WriteLine("param max must be bigger than param min. reset params, max = 255 & min = 0.");
                this.max = 255;
                this.min = 0;
            }
        }

        private string FormatPercent(int value) {
            return string.Format("{0}.{1}%", value / 10, value % 10);
        }
    }
}
