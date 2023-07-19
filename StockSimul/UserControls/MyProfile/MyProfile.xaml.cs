using Newtonsoft.Json;
using StockSimul.Scripts.Managers;
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
using static StockSimul.UserControls.MyProfile.MyProfile;

namespace StockSimul.UserControls.MyProfile
{
    /// <summary>
    /// MyProfile.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyProfile : UserControl
    {


        public MyProfile()
        {
            InitializeComponent();

            this.Loaded += MyProfile_Loaded;
        }

        private void MyProfile_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = MyProfileManager.Instance;
            this.Loaded -= MyProfile_Loaded;
        }
    }
}
