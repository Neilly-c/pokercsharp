using mainsource.cfrplus;
using pokercsharp.log;
using pokercsharp.mainsource;
using pokercsharp.mainsource.appendix;
using pokercsharp.mainsource.cfrplus;
using pokercsharp.ui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private Grid[] grids;
        private GraphBox[][][] boxes;

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
            grids = new Grid[3] { handGridSB, handGridBB, handGridSB2 };
            boxes = new GraphBox[grids.Length][][];
            for (int i = 0; i < boxes.Length; ++i) {
                boxes[i] = new GraphBox[Constants.CARDVALUE_LEN][];
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    boxes[i][j] = new GraphBox[Constants.CARDVALUE_LEN];
                }
            }
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
            isCFRRunning = false;
            runButton.Content = "Run";
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

            foreach (Grid grid in new Grid[] { handGrid, handGridSB, handGridBB, handGridSB2 }) {

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
                    handGrid.Children.Add(block);
                }
            }

            for (int i = 0; i < grids.Length; ++i) {
                for(int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    for(int k = 0; k < Constants.CARDVALUE_LEN; ++k) {
                        boxes[i][j][k] = new GraphBox();
                        Grid.SetRow(boxes[i][j][k], j + 1);
                        Grid.SetColumn(boxes[i][j][k], k + 1);
                        grids[i].Children.Add(boxes[i][j][k]);
                    }
                }
            }

        }

        private void ApplyCFRToView(Dictionary<string, Node> nodeMap) {

            double[][] strategyProb = new double[3][] { new double[3], new double[2], new double[2] };
            foreach (string key in nodeMap.Keys) {
                Boolean isSB = key.Length % 2 == 1;
                int gridNum = key.Length - 3;
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

                double[] avgStrategy = nodeMap[key].GetAverageStrategy();
                for (int i = 0; i < avgStrategy.Length; ++i) {
                    strategyProb[key.Length - 3][i] += avgStrategy[i] * combination;
                }
                boxes[gridNum][a][b].setValue(nodeMap[key].GetAverageStrategy());
            }

            ObservableCollection<ObservableInfo> collection = new ObservableCollection<ObservableInfo>();
            for (int i = 0; i < strategyProb.Length; ++i) {
                for (int j = 0; j < strategyProb[i].Length; ++j) {
                    collection.Add(new ObservableInfo {
                        name = i + ", " + j,
                        value = FormatPercent((int)(strategyProb[i][j] / Constants.COMBINATION * 1000 / (i == 0 ? 1 : strategyProb[i - 1].Length - 1)))
                    });
                }
            }
            collection.Add(new ObservableInfo { name = "average SB profit(bb)", value = cfr.utilPerIterations.ToString("F3") });
            summaryGrid.ItemsSource = collection;
        }

        private void InitCalcGrid() {
            handGridSB.Children.Clear();
            handGridBB.Children.Clear();
            handGridSB2.Children.Clear();

            foreach (Grid grid in new Grid[] { handGridSB, handGridBB, handGridSB2 }) {
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

            for (int i = 0; i < grids.Length; ++i) {
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    for (int k = 0; k < Constants.CARDVALUE_LEN; ++k) {
                        boxes[i][j][k] = new GraphBox();
                        Grid.SetRow(boxes[i][j][k], j + 1);
                        Grid.SetColumn(boxes[i][j][k], k + 1);
                        grids[i].Children.Add(boxes[i][j][k]);
                    }
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
