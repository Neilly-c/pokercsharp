using mainsource.cfrplus;
using pokercsharp.log;
using pokercsharp.mainsource;
using pokercsharp.mainsource.cfrplus;
using pokercsharp.mainsource.system;
using pokercsharp.mainsource.system.range;
using pokercsharp.ui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace pokercsharp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        AggregatedWinRateGrid agwr = new AggregatedWinRateGrid();
        CFRBase cfr = new CFRPlusPreflop();
        string[] prefix = new string[] { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

        private HeatBox[][] heatBoxes;
        private GraphBox[][] graphBoxes;
        private InputGraphBox[][] inputGraphBoxes;
        private bool isToCalcPostflop = false;
        private Thread worker;
        private WorkerThread wt;

        private ComboBox[][] actionSelectCombos;
        private Strategys strategys = new Strategys();
        const int PLAYERS = 6;
        readonly string[] actions = new string[3] { "Fold", "Call", "Raise" };

        public Dictionary<string, Node> nodeMap { get; set; }

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
            graphBoxes = new GraphBox[Constants.CARDVALUE_LEN][];
            heatBoxes = new HeatBox[Constants.CARDVALUE_LEN][];
            inputGraphBoxes = new InputGraphBox[Constants.CARDVALUE_LEN][];
            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                graphBoxes[i] = new GraphBox[Constants.CARDVALUE_LEN];
                heatBoxes[i] = new HeatBox[Constants.CARDVALUE_LEN];
                inputGraphBoxes[i] = new InputGraphBox[Constants.CARDVALUE_LEN];
            }
            actionSelectCombos = new ComboBox[PLAYERS][];
            for (int i = 0; i < PLAYERS; ++i) {
                actionSelectCombos[i] = new ComboBox[3];
            }
            InitView();
            wt = new WorkerThread(this, cfr, 0, 0, boardText.Text, false);
        }

        private void InitView() {

            foreach (Grid grid in new Grid[] { handGrid, handGridSolver, handGridRange }) {

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

            InitDefaultGrid();
            InitSolverGrid();
            InitRangeTab();
        }

        private void InitDefaultGrid() {
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
                    heatBoxes[a][b] = new HeatBox(0);
                    heatBoxes[a][b].SetMax(1000);
                    heatBoxes[a][b].SetText(0, (winCount * 1000 / (count * Constants._48C5 * 2)).ToString());
                    heatBoxes[a][b].SetText(1, count.ToString());
                    Grid.SetRow(heatBoxes[a][b], a + 1);
                    Grid.SetColumn(heatBoxes[a][b], b + 1);
                    handGrid.Children.Add(heatBoxes[a][b]);
                }
            }
        }

        private void InitSolverGrid() {
            handGridSolver.Children.Clear();

            foreach (Grid grid in new Grid[] { handGridSolver }) {
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

            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    graphBoxes[i][j] = new GraphBox();
                    Grid.SetRow(graphBoxes[i][j], i + 1);
                    Grid.SetColumn(graphBoxes[i][j], j + 1);
                    handGridSolver.Children.Add(graphBoxes[i][j]);
                }
            }
        }

        private void InitRangeTab() {
            for (int a = 0; a < Constants.CARDVALUE_LEN; ++a) {
                for (int b = 0; b < Constants.CARDVALUE_LEN; ++b) {
                    inputGraphBoxes[a][b] = new InputGraphBox();
                    Grid.SetRow(inputGraphBoxes[a][b], a + 1);
                    Grid.SetColumn(inputGraphBoxes[a][b], b + 1);
                    handGridRange.Children.Add(inputGraphBoxes[a][b]);
                    string str = "";
                    if (a == b) {
                        str = prefix[a] + prefix[b];
                        inputGraphBoxes[a][b].Tag = 6;
                    } else if (a > b) {
                        str = prefix[b] + prefix[a] + "o";
                        inputGraphBoxes[a][b].Tag = 12;
                    } else {
                        str = prefix[a] + prefix[b] + "s";
                        inputGraphBoxes[a][b].Tag = 4;
                    }
                    inputGraphBoxes[a][b].SetText(str);
                    inputGraphBoxes[a][b].SetValue(new double[3] { 1, 0, 0 });
                    inputGraphBoxes[a][b].MouseLeave += RangeUpdate;
                }
            }
            actionSelectCombos[0][0] = actSelectUTG1;
            actionSelectCombos[1][0] = actSelectHJ1;
            actionSelectCombos[2][0] = actSelectCO1;
            actionSelectCombos[3][0] = actSelectBTN1;
            actionSelectCombos[4][0] = actSelectSB1;
            actionSelectCombos[5][0] = actSelectBB1;
            for (int i = 0; i < PLAYERS; ++i) {
                actionSelectCombos[i][0].ItemsSource = i == 0 ? new string[] { "Action", "Fold", "Raise" } : new string[] { "-", "Action", "Fold", "Raise" };
                for (int j = 1; j < actionSelectCombos[i].Length; ++j) {
                    actionSelectCombos[i][j] = new ComboBox();
                    Grid.SetRow(actionSelectCombos[i][j], i + 1);
                    Grid.SetColumn(actionSelectCombos[i][j], j + 1);
                    actionselectGrid.Children.Add(actionSelectCombos[i][j]);
                    actionSelectCombos[i][j].ItemsSource = new string[] { "-", "Action", "Fold", "Call", "Raise" };
                    actionSelectCombos[i][j].IsEnabled = false;
                }
            }
            for (int i = 0; i < PLAYERS; ++i) {
                for (int j = 0; j < actionSelectCombos[i].Length; ++j) {
                    actionSelectCombos[i][j].SelectionChanged += OnActionSelectionChanged;
                    actionSelectCombos[i][j].SelectedIndex = 0;
                }
            }
        }

        private void OnActionSelectionChanged(object sender, SelectionChangedEventArgs e) {

            string key = GetActionKey();
            if (strategys.strategyByActionFacing.ContainsKey(key)) {
                RangeStrategy applyStrategy = strategys.strategyByActionFacing[key];
                for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                    for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                        Dictionary<string, double> dict = applyStrategy.GetStrategyDict(i, j);
                        double[] values = new double[5];
                        for (int a = 0; a < actions.Length; ++a) {
                            if (dict.ContainsKey(actions[a])) {
                                values[a] = dict[actions[a]];
                            }
                        }
                        inputGraphBoxes[i][j].SetValue(values);
                    }
                }
            } else {
                strategys.strategyByActionFacing.Add(key, new RangeStrategy());
                for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                    for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                        double[] values = new double[5] { 1, 0, 0, 0, 0 };
                        inputGraphBoxes[i][j].SetValue(values);
                    }
                }
            }
            while(key.Length >= 6) {
                key = key.Substring(0, key.Length - 6);
                for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                    for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                        if (strategys.strategyByActionFacing.ContainsKey(key)) {
                            RangeStrategy parentStrategy = strategys.strategyByActionFacing[key];
                            double prob = inputGraphBoxes[i][j].prob;
                            string actionKey = actionSelectCombos[key.Length / PLAYERS][key.Length % PLAYERS].SelectedItem.ToString();
                            prob *= parentStrategy.GetStrategyDict(i, j)[actionKey];
                            inputGraphBoxes[i][j].SetProb(prob);
                        } else {
                            strategys.strategyByActionFacing.Add(key, new RangeStrategy());
                        }

                    }
                }
            }
        }

        private void RangeUpdate(object sender, System.Windows.Input.MouseEventArgs e) {
            double[] valueSum = new double[5];
            for (int a = 0; a < Constants.CARDVALUE_LEN; ++a) {
                for (int b = 0; b < Constants.CARDVALUE_LEN; ++b) {
                    double[] values = inputGraphBoxes[a][b].values;
                    double parent = 0;
                    foreach (double d in values) {
                        parent += d;
                    }
                    if (parent > 0) {
                        for (int i = 0; i < values.Length; ++i) {
                            valueSum[i] += values[i] / parent * (int)(inputGraphBoxes[a][b].Tag);
                        }
                    }
                }
            }

            string key = GetActionKey();
            if (!strategys.strategyByActionFacing.ContainsKey(key)) {
                strategys.strategyByActionFacing.Add(key, new RangeStrategy());
            }
            RangeStrategy target = strategys.strategyByActionFacing[key];
            for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                    target.SetStrategy(i, j, inputGraphBoxes[i][j].values, actions);
                }
            }

            ObservableCollection<ObservableInfo> collection = new ObservableCollection<ObservableInfo>();
            for (int i = 0; i < valueSum.Length; ++i) {
                collection.Add(new ObservableInfo {
                    name = i.ToString(),
                    value = FormatPercent((int)(valueSum[i] / Constants.COMBINATION * 1000)),
                });
            }
            summaryGridRange.ItemsSource = collection;
        }

        private string GetActionKey() {
            string key = "";
            for (int i = 0; i < actionSelectCombos[0].Length; ++i) {
                for (int j = 0; j < PLAYERS; ++j) {
                    if (actionSelectCombos[j][i].SelectedIndex == 0 || actionSelectCombos[j][i].SelectedIndex == 1) {
                        break;
                    } else {
                        switch (actionSelectCombos[j][i].SelectedItem) {
                            case "Fold":
                                key += "f";
                                break;
                            case "Check":
                                key += "x";
                                break;
                            case "Call":
                                key += "c";
                                break;
                            case "Raise":
                                key += "r";
                                break;
                        }
                    }
                }
            }
            return key;
        }

        private TextBlock TextBlockInGrid(string value) {
            TextBlock block = new TextBlock();
            block.Text = value;
            block.FontSize = 20;
            block.VerticalAlignment = VerticalAlignment.Center;
            block.HorizontalAlignment = HorizontalAlignment.Center;
            return block;
        }

        private string FormatPercent(int value) {
            return string.Format("{0}.{1}%", value / 10, value % 10);
        }

        private void buildButton_Click(object sender, RoutedEventArgs e) {
            double stack = Int32.Parse(stackText.Text);
            BuildTree(stack, 1.5d);
            Console.WriteLine("Build Tree Completed");
        }

        private void preToggle_Click(object sender, RoutedEventArgs e) {
            isToCalcPostflop = (bool)(sender as ToggleButton).IsChecked;
            if (isToCalcPostflop) {
                cfr = new CFRPlusFLPostflop();
            } else {
                cfr = new CFRPlusPreflop();
            }
        }

        static bool isCFRRunning = false;

        private void runButton_Click(object sender, RoutedEventArgs e) {
            if (isCFRRunning) {
                isCFRRunning = false;
                runButton.Content = "Run";
            } else {
                isCFRRunning = true;
                runButton.Content = "Halt";
                double stack = Int32.Parse(stackText.Text);
                BuildTree(stack, 1.5d);
                Console.WriteLine("Activate CFR...");

                wt.cfr = cfr;
                wt.stack = stack;
                wt.iter = Int32.Parse(iterText.Text);
                wt.board = boardText.Text;
                wt.isToCalcPostflop = isToCalcPostflop;
                worker = new Thread(new ThreadStart(wt.ThreadASync));

                worker.Start();
                Console.WriteLine("Complete");
                isCFRRunning = false;
                runButton.Content = "Run";
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e) {      //今これじゃ止まらない
            Console.WriteLine("Reset");
            isCFRRunning = false;
            runButton.Content = "Run";
            cfr.RefreshNodeMap();
            InitSolverGrid();
        }

        private void BuildTree(double stack, double pot) {
            List<NodeGroup> nodeGroups = new List<NodeGroup>();
            NodeGroup root = cfr.CreateNode(ref nodeGroups, "", stack, pot, isToCalcPostflop ? 0 : 0.5d);
            ObservableCollection<NodeGroup> ocNodes = new ObservableCollection<NodeGroup>();
            ocNodes.Add(root);
            nodeTree.ItemsSource = ocNodes;
            nodeTree.SelectedItemChanged += NodeTree_SelectedItemChanged;
        }

        private void NodeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            string selectedHistory = ((NodeGroup)(e.NewValue)).history;
            foreach (GraphBox[] tmp in graphBoxes) {
                foreach (GraphBox gb in tmp) {
                    gb.nodes.Clear();
                }
            }
            Console.WriteLine(selectedHistory);

            Dictionary<string, Node> nodeMap = cfr.nodeMap;     //これ要る？
            if (isToCalcPostflop) {
                foreach (string key in nodeMap.Keys) {
                    if ((key.Length == 4 && selectedHistory.Equals("")) || (key.Length >= 5 && key.Substring(4).Equals(selectedHistory))) {
                        int a = Constants.handKey[key[0]],
                            b = Constants.handKey[key[2]];
                        if (a != b) {
                            if (!key[1].Equals(key[3])) {
                                int temp = a; a = b; b = temp;
                            }
                        }
                        graphBoxes[a][b].AddNode(nodeMap[key]);
                    }
                }
            } else {
                foreach (string key in nodeMap.Keys) {
                    if ((key.Length == 3 && selectedHistory.Equals("")) || (key.Length >= 4 && key.Substring(3).Equals(selectedHistory))) {
                        int a = Constants.handKey[key[0]],
                            b = Constants.handKey[key[1]];
                        if (a != b) {
                            if (key[2].Equals('o')) {
                                int temp = a; a = b; b = temp;
                            }
                        }
                        graphBoxes[a][b].AddNode(nodeMap[key]);
                    }
                }
            }
            foreach (GraphBox[] tmp in graphBoxes) {
                foreach (GraphBox gb in tmp) {
                    gb.Render();
                }
            }
        }

        private void ApplyCFRToView(Dictionary<string, Node> nodeMap) {

            if (isToCalcPostflop) {

                double[] strategyProb = new double[5];
                foreach (string key in nodeMap.Keys) {
                    int a = Constants.handKey[key[0]],
                        b = Constants.handKey[key[2]];
                    if (a != b) {
                        if (!key[1].Equals(key[3])) {
                            int temp = a; a = b; b = temp;
                        }
                    }

                    double[] avgStrategy = nodeMap[key].GetAverageStrategy();
                    for (int i = 0; i < avgStrategy.Length; ++i) {
                        strategyProb[i] += avgStrategy[i];
                    }
                    graphBoxes[a][b].AddNode(nodeMap[key]);
                }

                ObservableCollection<ObservableInfo> collection = new ObservableCollection<ObservableInfo>();
                for (int i = 0; i < strategyProb.Length; ++i) {
                    collection.Add(new ObservableInfo {
                        name = i.ToString(), value = FormatPercent((int)(strategyProb[i] / Constants.COMBINATION * 1000))
                    });
                }
                collection.Add(new ObservableInfo { name = "average SB profit(bb)", value = cfr.utilPerIterations.ToString("F3") });
                summaryGrid.ItemsSource = collection;

            } else {

                double[][] strategyProb = new double[3][] { new double[3], new double[2], new double[2] };
                foreach (string key in nodeMap.Keys) {
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
                    graphBoxes[a][b].AddNode(nodeMap[key]);
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
        }


    }
}
