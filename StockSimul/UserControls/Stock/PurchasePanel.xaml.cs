using StockSimul.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static StockSimul.Scripts.Managers.MyProfileManager;

namespace StockSimul.UserControls.Stock
{
    /// <summary>
    /// PurchasePanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PurchasePanel : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ProfileDisplay> profileDisplay;
        public ObservableCollection<ProfileDisplay> ProfileDisplay {
            get {
                return profileDisplay;
            }
            set { 
                if (profileDisplay != value)
                {
                    profileDisplay = value;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MyPurchaseDisplay> purchasedStockList;
        public ObservableCollection<MyPurchaseDisplay> PurchasedStockList
        {
            get
            {
                return purchasedStockList;
            }
            set
            {
                if (purchasedStockList != value)
                {
                    purchasedStockList = value;
                }
                OnPropertyChanged();
            }
        }

        public PurchasePanel()
        {
            InitializeComponent();
            this.DataContext= this;

            ProfileDisplay = MyProfileManager.Instance.profileDisplay;
            PurchasedStockList = MyProfileManager.Instance.myPurchaseDisplay;


        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyPurchaseDisplay item = null;
            if(e.AddedItems.Count != 0)
            {
                item = e.AddedItems[0] as MyPurchaseDisplay;
            }
            else if (e.RemovedItems.Count != 0)
            {
                item = e.RemovedItems[0] as MyPurchaseDisplay;
            }
            item.IsSelected = !item.IsSelected;
        }
    }
}
