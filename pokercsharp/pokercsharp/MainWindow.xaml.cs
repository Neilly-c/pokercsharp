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
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
        private Button[] actionButtons;
        private RadioButton[] actionRadios;
        private InputGraphBox[][] inputGraphBoxes;
        private bool isToCalcPostflop = false;
        private Thread worker;
        private WorkerThread wt;

        const int PLAYERS = 6;
        private Strategys strategys = new Strategys();
        ActionKeyParser akp = new ActionKeyParser(PLAYERS, 5);
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
            actionButtons = new Button[PLAYERS] { actUTG, actHJ, actCO, actBTN, actSB, actBB };
            actionRadios = new RadioButton[PLAYERS] { radioUTG, radioHJ, radioCO, radioBTN, radioSB, radioBB };
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
                    inputGraphBoxes[a][b].MouseLeftButtonDown += SetHandStrategy1;
                    inputGraphBoxes[a][b].MouseRightButtonDown += SetHandStrategy2;
                }
            }
        }

        private void SetHandStrategy1(object sender, MouseButtonEventArgs e) {
            double[] decision = new double[3];
            if ((bool)clickDefFold.IsChecked) {
                decision[0] = 100;
            } else if ((bool)clickDefCall.IsChecked) {
                decision[1] = 100;
            } else if ((bool)clickDefRaise.IsChecked) {
                decision[2] = 100;
            }
            (sender as InputGraphBox).SetValue(decision);
            RangeUpdate();
        }

        private void SetHandStrategy2(object sender, MouseButtonEventArgs e) {
            double[] decision = new double[3];
            decision[0] = 100;
            (sender as InputGraphBox).SetValue(decision);
            RangeUpdate();
        }
        
        ///<summary>
        ///親レンジ含め選択を修正する
        ///</summary>
        private void SetHandStrategy3(object sender, MouseButtonEventArgs e){
            int row = 0, col = 0;
            while((sender as InputGraphBox).Equals(inputGraphBoxes[row][col])){
                ++col;
                if(col >= Constants.CARDVALUE_LEN){
                    col = 0;
                    ++row;
                }
            }       //発見できなかったときの処理がない(そんなことは起きないはずだが)
            double[] decision = new double[3];
            decision[0] = 100;
            (sender as InputGraphBox).SetValue(decision);
            
            string key = GetActionKey();
            while (key.Length >= 6 + 2) {
                char prevActionChar = parentKey[^6];
                key = key[0..^6];
                double[] dec = new double[3];
                switch(prevActionChar){
                    case 'f':
                        dec[0] = 100;
                        break;
                    case 'c':
                        dec[1] = 100;
                        breal;
                    case 'r':
                    default:
                        dec[2] = 100;
                        break;
                }
                OneRangeUpdate(key, row, col, dec);
            }
            RangeUpdate();
        }

        private void ApplyKey_Click(object sender, RoutedEventArgs e) {
            ChangeVisualizedRange(GetActionKey());
        }

        /// <summary>
        /// レンジ選択の変更があるごとに呼び出される
        /// RangeStrategyを保存し直す
        /// </summary>
        private void RangeUpdate() {
            double[] valueSum = new double[5];
            double childSum = 0;
            for (int a = 0; a < Constants.CARDVALUE_LEN; ++a) {
                for (int b = 0; b < Constants.CARDVALUE_LEN; ++b) {
                    childSum += (int)(inputGraphBoxes[a][b].Tag) * inputGraphBoxes[a][b].prob;
                    double[] values = inputGraphBoxes[a][b].values;
                    double parent = 0;
                    foreach (double d in values) {
                        parent += d;
                    }
                    if (parent > 0) {
                        for (int i = 0; i < values.Length; ++i) {
                            valueSum[i] += values[i] / parent * (int)(inputGraphBoxes[a][b].Tag) * inputGraphBoxes[a][b].prob;
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

            ObservableCollection<RangeInfo> collection = new ObservableCollection<RangeInfo>();
            for (int i = 0; i < actions.Length; ++i) {
                collection.Add(new RangeInfo {
                    name = actions[i],
                    value = FormatPercent((int)(valueSum[i] / Constants.COMBINATION * 1000)),
                    childValue = FormatPercent((int)(valueSum[i] / childSum * 1000)),
                });
            }
            summaryGridRange.ItemsSource = collection;
        }
        
        ///<summary>
        ///RangeUpdateを1つのハンドに対してだけ適用する
        ///</summary>
        private void OneRandeUpdate(string key, int row, int col, double[] decision){
            if (!strategys.strategyByActionFacing.ContainsKey(key)) {
                Debug.WriteLine("Creating new strategy is not a safe method: OneRangeUpdate()");
                strategys.strategyByActionFacing.Add(key, new RangeStrategy());
            }
            RangeStrategy target = strategys.strategyByActionFacing[key];
            target.SetStrategy(row, col, decision, actions);
        }

        private void ChangeVisualizedRange(string newKey) {

            if (strategys.strategyByActionFacing.ContainsKey(newKey)) {
                RangeStrategy applyStrategy = strategys.strategyByActionFacing[newKey];
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
                        inputGraphBoxes[i][j].SetProb(1);
                    }
                }
            } else {
                strategys.strategyByActionFacing.Add(newKey, new RangeStrategy());
                for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                    for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                        double[] values = new double[5] { 1, 0, 0, 0, 0 };
                        inputGraphBoxes[i][j].SetValue(values);
                        inputGraphBoxes[i][j].SetProb(1);
                    }
                }
            }
            string parentKey = newKey;
            while (parentKey.Length >= 6 + 2) {       //prefix"PF"の2
                char prevActionChar = parentKey[^6];
                string actionKey = actions[0];
                switch (prevActionChar) {
                    case 'f':
                        actionKey = actions[0];
                        break;
                    case 'c':
                        actionKey = actions[1];
                        break;
                    case 'r':
                    default:
                        actionKey = actions[2];
                        break;
                }
                parentKey = parentKey[0..^6];
                for (int i = 0; i < Constants.CARDVALUE_LEN; ++i) {
                    for (int j = 0; j < Constants.CARDVALUE_LEN; ++j) {
                        if (strategys.strategyByActionFacing.ContainsKey(parentKey)) {
                            RangeStrategy parentStrategy = strategys.strategyByActionFacing[parentKey];
                            double prob = inputGraphBoxes[i][j].prob;
                            if (parentStrategy.GetStrategyDict(i, j).ContainsKey(actionKey)) {
                                prob *= parentStrategy.GetStrategyDict(i, j)[actionKey];
                            } else {
                                Debug.WriteLine("No such Key");
                                prob *= 0;
                            }
                            inputGraphBoxes[i][j].SetProb(prob);
                        } else {
                            strategys.strategyByActionFacing.Add(parentKey, new RangeStrategy());
                        }
                    }
                }
            }
        }

        private string GetActionKey() {
            string key = "PF" + strKey.Text;
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

                ObservableCollection<RangeInfo> collection = new ObservableCollection<RangeInfo>();
                for (int i = 0; i < strategyProb.Length; ++i) {
                    collection.Add(new RangeInfo {
                        name = i.ToString(), value = FormatPercent((int)(strategyProb[i] / Constants.COMBINATION * 1000))
                    });
                }
                collection.Add(new RangeInfo { name = "average SB profit(bb)", value = cfr.utilPerIterations.ToString("F3") });
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

                ObservableCollection<RangeInfo> collection = new ObservableCollection<RangeInfo>();
                for (int i = 0; i < strategyProb.Length; ++i) {
                    for (int j = 0; j < strategyProb[i].Length; ++j) {
                        collection.Add(new RangeInfo {
                            name = i + ", " + j,
                            value = FormatPercent((int)(strategyProb[i][j] / Constants.COMBINATION * 1000 / (i == 0 ? 1 : strategyProb[i - 1].Length - 1)))
                        });
                    }
                }
                collection.Add(new RangeInfo { name = "average SB profit(bb)", value = cfr.utilPerIterations.ToString("F3") });
                summaryGrid.ItemsSource = collection;

            }
        }
    }
}
