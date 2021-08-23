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

        const int FULL_GRID_SIZE = 1327;
        const int _48C5 = 1712304;
        string[] hands;
        Grid fullGrid;

        public MainWindow() {
            InitializeComponent();
            Debug.WriteLine("Hello!");

            ScrollViewer scrollViewer = this.FindName("baseScroll") as ScrollViewer;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            fullGrid = new Grid();
            fullGrid.Width = FULL_GRID_SIZE * 20;
            fullGrid.Height = FULL_GRID_SIZE * 20;
            fullGrid.HorizontalAlignment = HorizontalAlignment.Left;
            fullGrid.VerticalAlignment = VerticalAlignment.Top;
            fullGrid.ShowGridLines = true;
            for (int i = 0; i < FULL_GRID_SIZE; ++i) {
                ColumnDefinition colDef = new ColumnDefinition();
                RowDefinition rowDef = new RowDefinition();
                fullGrid.ColumnDefinitions.Add(colDef);
                fullGrid.RowDefinitions.Add(rowDef);
            }

            ReadFile();

            for (int i = 1; i < FULL_GRID_SIZE; ++i) {
                TextBlock txt1 = new TextBlock(), txt2 = new TextBlock();
                txt1.Text = hands[i - 1];
                txt2.Text = hands[i - 1];
                txt1.FontSize = 8;
                txt2.FontSize = 8;
                Grid.SetColumn(txt1, i);
                Grid.SetColumn(txt2, 0);
                Grid.SetRow(txt1, 0);
                Grid.SetRow(txt2, i);
                fullGrid.Children.Add(txt1);
                fullGrid.Children.Add(txt2);
            }

            scrollViewer.Content = fullGrid;
            Debug.WriteLine("Hello!");

            /*
            FinalHandsDict finalHandsDict = new FinalHandsDict();
            finalHandsDict.Init();
            WinRateGrid winRateGrid = new WinRateGrid();
            winRateGrid.Init();
            */
        }

        private void ReadFile() {
            IEnumerable<string> ies = File.ReadLines("D:\\Csharp\\pokercsharp\\winRateGrid.txt");
            foreach (string s in ies) {
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGridex.txt", s.Trim('{').Trim('}'));
                File.AppendAllText(@"D:\Csharp\pokercsharp\winRateGridex.txt", Environment.NewLine);
            }
            string handstr = ies.First();
            hands = handstr.Split(',');
            int rowIter = 0;
            /*
            foreach(string s in ies) {
                if (s.Contains('{')) {
                    ++rowIter;
                    string[] line = s.Trim('{').Trim('}').Split(',');
                    for(int i = 1; i < FULL_GRID_SIZE - 1; ++i) {
                        Rectangle rect = new Rectangle();
                        if(Int32.Parse(line[i - 1]) == -1) {
                            rect.Fill = Brushes.Gray;
                        } else if (Int32.Parse(line[i - 1]) == 0) {
                            rect.Fill = Brushes.Black;
                        } else if((float)Int32.Parse(line[i - 1])/(float)_48C5 == 0.5f) {
                            rect.Fill = Brushes.AliceBlue;
                        } else if ((float)Int32.Parse(line[i - 1]) / (float)_48C5 > 0.5f) {
                            rect.Fill = Brushes.Blue;
                        } else {
                            rect.Fill = Brushes.Red;
                        }
                        Grid.SetColumn(rect, i);
                        Grid.SetRow(rect, rowIter);
                        fullGrid.Children.Add(rect);
                    }
                }
            }*/
        }
    }
}
