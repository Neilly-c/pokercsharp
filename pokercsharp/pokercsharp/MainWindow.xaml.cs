using mainsource.cfrplus;
using pokercsharp.mainsource;
using pokercsharp.mainsource.appendix;
using pokercsharp.mainsource.cfrplus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pokercsharp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        AggregatedWinRateGrid agwr = new AggregatedWinRateGrid();
        string[] prefix = new string[] { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

        public MainWindow() {
            InitializeComponent();
            Debug.WriteLine("Hello!");
            /*
            FinalHandsDict finalHandsDict = new FinalHandsDict();
            finalHandsDict.Init();
            WinRateGrid winRateGrid = new WinRateGrid();
            winRateGrid.Init();
            winRateGrid.ReadCSVofGrid();
            */
            agwr.Init();
            InitView();
            CFRPlus cfr = new CFRPlus();
            cfr.Train(100000);
        }

        private void InitView() {
            Grid handGrid = this.FindName("handGrid") as Grid;
            handGrid.ShowGridLines = true;
            for (int i = 0; i < Constants.CARDVALUE_LEN + 1; ++i) {
                ColumnDefinition colDef = new ColumnDefinition();
                RowDefinition rowDef = new RowDefinition();
                handGrid.ColumnDefinitions.Add(colDef);
                handGrid.RowDefinitions.Add(rowDef);
            }

            for (int i = 1; i < Constants.CARDVALUE_LEN + 1; ++i) {
                TextBlock txt1 = TextBlockInGrid(prefix[i - 1]), txt2 = TextBlockInGrid(prefix[i - 1]);
                Grid.SetColumn(txt1, i);
                Grid.SetColumn(txt2, 0);
                Grid.SetRow(txt1, 0);
                Grid.SetRow(txt2, i);
                handGrid.Children.Add(txt1);
                handGrid.Children.Add(txt2);
            }

            int[][][][] digest_grid = agwr.Get_digest_grid();
            int[][][][] digest_count = agwr.Get_digest_count();

            for (int a = 0; a < Constants.CARDVALUE_LEN; ++a) {
                for (int b = 0; b < Constants.CARDVALUE_LEN; ++b) {
                    long winCount = 0;
                    long count = 0;
                    for (int c = 0; c < Constants.CARDVALUE_LEN; ++c) {
                        for (int d = 0; d < Constants.CARDVALUE_LEN; ++d) {
                            winCount += digest_grid[a][b][c][d];
                            count += digest_count[a][b][c][d];
                        }
                    }
                    TextBlock block = TextBlockWithColorChart((winCount * 1000 / (count * Constants._48C5 * 2)).ToString(),
                        count.ToString());
                    Grid.SetRow(block, a + 1);
                    Grid.SetColumn(block, b + 1);
                    handGrid.Children.Add(block);
                }
            }

            Debug.WriteLine("Hello!");

        }

        private TextBlock TextBlockInGrid(string value) {
            TextBlock block = new TextBlock();
            block.Text = value;
            block.FontSize = 20;
            block.VerticalAlignment = VerticalAlignment.Center;
            block.HorizontalAlignment = HorizontalAlignment.Center;
            return block;
        }

        private TextBlock TextBlockWithColorChart(string value, params string[] str) {
            TextBlock block = new TextBlock();
            byte b = (byte)(Int32.Parse(value) / 5);
            string text = "";
            for (int i = 0; i < str.Length; ++i) {
                text = text + "\n" + str[i];
            }
            block.Text = string.Format("{0}.{1}%", value.Substring(0,2), value.Substring(2,1)) + text;
            block.FontSize = 16;
            block.VerticalAlignment = VerticalAlignment.Stretch;
            block.HorizontalAlignment = HorizontalAlignment.Stretch;
            block.TextAlignment = TextAlignment.Center;
            block.Background = new SolidColorBrush(Color.FromArgb(0xFF, (byte)(0xFF - b), (byte)(0xFF - b / 2), b));
            return block;
        }
    }
}
