using mainsource.cfrplus;
using pokercsharp.log;
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
using System.Windows.Threading;

namespace pokercsharp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        AggregatedWinRateGrid agwr = new AggregatedWinRateGrid();
        CFRPlus cfr = new CFRPlus();
        string[] prefix = new string[] { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };
        Grid wrGrid, sbGrid, bbGrid;

        public MainWindow() {
            InitializeComponent();
            Console.SetOut(new ControlWriter(logBox));
            Console.WriteLine("Component Initialized");
            /*
            FinalHandsDict finalHandsDict = new FinalHandsDict();
            finalHandsDict.Init();
            WinRateGrid winRateGrid = new WinRateGrid();
            winRateGrid.Init();
            winRateGrid.ReadCSVofGrid();
            */

            agwr.Init();
            InitView();
        }

        static bool isCFRRunning = false;

        private void runButton_Click(object sender, RoutedEventArgs e) {
            if (isCFRRunning) {
                isCFRRunning = false;
                runButton.Content = "Run";
            } else {
                isCFRRunning = true;
                runButton.Content = "Halt";
                Console.WriteLine("Activate CFR...");
                int iter = Int32.Parse(iterText.Text);
                for (int i = 0; i < (iter >= 6 ? Math.Pow(10, iter - 6) : 1); ++i) {
                    if (!isCFRRunning) {
                        break;
                    }
                    Dictionary<string, Node> nodeMap = cfr.Run(1000000, Int32.Parse(stackText.Text));
                    ApplyCFRToView(nodeMap);
                    Console.WriteLine("Iteration " + i + ",000,000");
                    DoEvents();
                }
                Console.WriteLine("Complete");
                isCFRRunning = false;
                runButton.Content = "Run";
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine("Reset");
            cfr.refreshNodeMap();
            InitCalcGrid();
        }

        private void DoEvents() {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(ExitFrames);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        private object ExitFrames(object obj) {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }

        private void InitView() {
            wrGrid = this.FindName("handGrid") as Grid;
            sbGrid = this.FindName("handGridSB") as Grid;
            bbGrid = this.FindName("handGridBB") as Grid;

            foreach (Grid grid in new Grid[] { wrGrid, sbGrid, bbGrid }) {

                grid.ShowGridLines = true;
                for (int i = 0; i < Constants.CARDVALUE_LEN + 1; ++i) {
                    ColumnDefinition colDef = new ColumnDefinition();
                    RowDefinition rowDef = new RowDefinition();
                    grid.ColumnDefinitions.Add(colDef);
                    grid.RowDefinitions.Add(rowDef);
                }

                for (int i = 1; i < Constants.CARDVALUE_LEN + 1; ++i) {
                    TextBlock txt1 = TextBlockInGrid(prefix[i - 1]), txt2 = TextBlockInGrid(prefix[i - 1]);
                    Grid.SetColumn(txt1, i);
                    Grid.SetColumn(txt2, 0);
                    Grid.SetRow(txt1, 0);
                    Grid.SetRow(txt2, i);
                    grid.Children.Add(txt1);
                    grid.Children.Add(txt2);
                }
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
                    wrGrid.Children.Add(block);
                }
            }

        }

        private void ApplyCFRToView(Dictionary<string, Node> nodeMap) {
            InitCalcGrid();

            double sbPush = 0, bbPush = 0;
            foreach (string key in nodeMap.Keys) {
                Boolean isSB = !key[key.Length - 1].Equals('p');
                Grid gridApplyTo = isSB ? sbGrid : bbGrid;
                int a = Constants.handKey[key[0]],
                    b = Constants.handKey[key[1]];
                int combination;
                if (a != b) {
                    if (key[2].Equals('o')) {
                        int temp = a; a = b; b = temp;
                        combination = 12;
                    } else {
                        combination = 4;
                    }
                } else {
                    combination = 6;
                }
                if (isSB) {
                    sbPush += nodeMap[key].GetAverageStrategy()[1] * combination;
                } else {
                    bbPush += nodeMap[key].GetAverageStrategy()[1] * combination;
                }
                TextBlock block = TextBlockWithColorChart(((int)(nodeMap[key].GetAverageStrategy()[1] * 1000)).ToString());
                Grid.SetRow(block, a + 1);
                Grid.SetColumn(block, b + 1);
                gridApplyTo.Children.Add(block);
            }
            sbPushPercent.Text = FormatPercent((int)(sbPush / Constants.COMBINATION * 1000));
            bbPushPercent.Text = FormatPercent((int)(bbPush / Constants.COMBINATION * 1000));
            avgSBprofit.Text = cfr.utilPerIterations.ToString("F3");
        }

        private void InitCalcGrid() {
            sbGrid.Children.Clear();
            bbGrid.Children.Clear();

            foreach (Grid grid in new Grid[] { sbGrid, bbGrid }) {

                for (int i = 1; i < Constants.CARDVALUE_LEN + 1; ++i) {
                    TextBlock txt1 = TextBlockInGrid(prefix[i - 1]), txt2 = TextBlockInGrid(prefix[i - 1]);
                    Grid.SetColumn(txt1, i);
                    Grid.SetColumn(txt2, 0);
                    Grid.SetRow(txt1, 0);
                    Grid.SetRow(txt2, i);
                    grid.Children.Add(txt1);
                    grid.Children.Add(txt2);
                }
            }
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
            block.Text = FormatPercent(Int32.Parse(value)) + text;
            block.FontSize = 16;
            block.VerticalAlignment = VerticalAlignment.Stretch;
            block.HorizontalAlignment = HorizontalAlignment.Stretch;
            block.TextAlignment = TextAlignment.Center;
            block.Background = new SolidColorBrush(Color.FromArgb(0xFF, (byte)(0xFF - b), (byte)(0xFF - b / 2), b));
            return block;
        }

        private string FormatPercent(int value) {
            return string.Format("{0}.{1}%", value / 10, value % 10);
        }
    }
}
