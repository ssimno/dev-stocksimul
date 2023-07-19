using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static StockSimul.Scripts.Managers.StockManager;

namespace StockSimul.UserControls.Stock
{
    /// <summary>
    /// StockItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StockItem : UserControl
    {
        public StockItemInfo StockInfo { get; set; }

        public StockItem(StockItemInfo stockItemInfo)
        {
            InitializeComponent();

            StockInfo = stockItemInfo;

            this.DataContext = StockInfo;
        }

        
    }
}
