using pokercsharp.mainsource.appendix;
using pokercsharp.mainsource.cfrplus;
using System;
using System.Collections.Generic;
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

namespace pokercsharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("Hello!");

            KuhnPoker kp = new KuhnPoker();

            /*
            FinalHandsDict finalHandsDict = new FinalHandsDict();
            finalHandsDict.Init();
            WinRateGrid winRateGrid = new WinRateGrid();
            winRateGrid.Init();
            */
        }
    }
}
