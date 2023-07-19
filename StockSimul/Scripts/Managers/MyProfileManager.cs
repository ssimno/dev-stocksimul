using CommonUtilities;
using StockSimul.Scripts.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static StockSimul.Scripts.Managers.MyProfileManager;
using Brush = System.Windows.Media.Brush;

namespace StockSimul.Scripts.Managers
{
    public class MyProfileManager : Singleton<MyProfileManager>
    {
        public class MyProfileInfos : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public class PurchasedItemInfo
            {
                public int id { get; set; }
                public float purchasedPrice { get; set; }
                public int purchasedAmount { get; set; }
            }
            public List<PurchasedItemInfo> purchasedItemInfos { get; set; }

            private float seedMoney;
            public float SeedMoney { 
                get => seedMoney; 
                set 
                { 
                    seedMoney= value;
                    OnPropertyChanged();
                } 
            }

            public MyProfileInfos()
            {
                purchasedItemInfos = new List<PurchasedItemInfo>();
            }

        }

        public class ProfileDisplay : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private float totalPurchasePrice;
            /// <summary>
            /// 매입금액
            /// </summary>
            public float TotalPurchasePrice
            {
                get { return totalPurchasePrice; }
                set
                {
                    totalPurchasePrice = value;
                    ChangeEvaluationProfitAndLoss();
                    OnPropertyChanged();
                }
            }

            private float totalEvaluatedPrice;
            /// <summary>
            /// 평가금액
            /// </summary>
            public float TotalEvaluatedPrice
            {
                get { return totalEvaluatedPrice; }
                set
                {
                    totalEvaluatedPrice = value;
                    ChangeEvaluationProfitAndLoss();
                    OnPropertyChanged();
                }
            }

            private float evaluationProfitAndLoss;
            /// <summary>
            /// 평가손익
            /// </summary>
            public float EvaluationProfitAndLoss
            {
                get { return evaluationProfitAndLoss; }
                set
                {
                    if (value > 0)
                        ViewFontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 105, 97));
                    else if (value < 0)
                        ViewFontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 149, 237));
                    else
                        ViewFontColor = System.Windows.Media.Brushes.Black;

                    evaluationProfitAndLoss = value;
                    OnPropertyChanged();
                }
            }

            private float profitRate;
            /// <summary>
            /// 수익률
            /// </summary>
            public float ProfitRate
            {
                get { return profitRate; }
                set
                {
                    profitRate = value;
                    OnPropertyChanged();
                }
            }

            private float realizedProfitAndLoss;
            /// <summary>
            /// 실현손익
            /// </summary>
            public float RealizedProfitAndLoss
            {
                get { return realizedProfitAndLoss; }
                set
                {
                    realizedProfitAndLoss = value;
                    OnPropertyChanged();
                }
            }

            private float totalProfitAndLoss;
            /// <summary>
            /// 일괄매도
            /// </summary>
            public float TotalProfitAndLoss
            {
                get { return totalProfitAndLoss; }
                set
                {
                    totalProfitAndLoss = value;
                    OnPropertyChanged();
                }
            }

            private Brush viewFontColor = System.Windows.Media.Brushes.Black;
            /// <summary>
            /// 주식아이템 시각화 - 주가 등락에 따른 색 변경
            /// </summary>
            public Brush ViewFontColor
            {
                get
                {
                    return viewFontColor;
                }
                set
                {
                    viewFontColor = value;
                    OnPropertyChanged(nameof(ViewFontColor));
                }
            }

            private ICommand sellAll;
            /// <summary>
            /// 
            /// </summary>
            public ICommand SellAll
            {
                get { return sellAll; }
                set
                {
                    sellAll = value;
                    OnPropertyChanged();
                }
            }

            private void ChangeEvaluationProfitAndLoss()
            {
                EvaluationProfitAndLoss = TotalEvaluatedPrice - TotalPurchasePrice;
                ProfitRate = (TotalEvaluatedPrice / TotalPurchasePrice) * 100 - 100;
                if (float.IsNaN(ProfitRate))
                    ProfitRate = 0;
            }

        }

        public class MyPurchaseDisplay : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public int id { get; set; }

            private bool isSelected;
            /// <summary>
            /// 선택
            /// </summary>
            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }

            private string stockName;
            /// <summary>
            /// 종목명
            /// </summary>
            public string StockName
            {
                get { return stockName; }
                set
                {
                    stockName = value;
                    OnPropertyChanged();
                }
            }

            private int quantity;
            /// <summary>
            /// 수량
            /// </summary>
            public int Quantity
            {
                get { return quantity; }
                set
                {
                    quantity = value;
                    ChangePurchasePrice();
                    OnPropertyChanged();
                }
            }

            private float purchasePrice;
            /// <summary>
            /// 매입단가
            /// </summary>
            public float PurchasePrice
            {
                get { return purchasePrice; }
                set
                {
                    purchasePrice = value;
                    OnPropertyChanged();
                }
            }

            private float purchaseAmount;
            /// <summary>
            /// 매입금액
            /// </summary>
            public float PurchaseAmount
            {
                get { return purchaseAmount; }
                set
                {
                    purchaseAmount = value;
                    ChangePurchasePrice();
                    OnPropertyChanged();
                }
            }

            private float currentPrice;
            /// <summary>
            /// 현재가
            /// </summary>
            public float CurrentPrice
            {
                get { return currentPrice; }
                set
                {
                    if (value > PurchasePrice)
                        ViewFontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 105, 97));
                    else if (value < PurchasePrice)
                        ViewFontColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 149, 237));
                    else
                        ViewFontColor = System.Windows.Media.Brushes.Black;

                    currentPrice = value;
                    OnPropertyChanged();
                }
            }

            private float evaluatedAmount;
            /// <summary>
            /// 평가금액
            /// </summary>
            public float EvaluatedAmount
            {
                get { return evaluatedAmount; }
                set
                {
                    evaluatedAmount = value;
                    OnPropertyChanged();
                }
            }

            private float netProfit;
            /// <summary>
            /// 평가순익
            /// </summary>
            public float NetProfit
            {
                get { return netProfit; }
                set
                {
                    netProfit = value;
                    OnPropertyChanged();
                }
            }

            private Brush viewFontColor = System.Windows.Media.Brushes.Black;
            /// <summary>
            /// 주식아이템 시각화 - 주가 등락에 따른 색 변경
            /// </summary>
            public Brush ViewFontColor
            {
                get
                {
                    return viewFontColor;
                }
                set
                {
                    viewFontColor = value;
                    OnPropertyChanged(nameof(ViewFontColor));
                }
            }

            private void ChangePurchasePrice()
            {
                PurchasePrice = PurchaseAmount / Quantity;
            }
        }


        public ObservableCollection<ProfileDisplay> profileDisplay;
        public ObservableCollection<MyPurchaseDisplay> myPurchaseDisplay;
        public MyProfileInfos myProfileInfos { get; set; }

        public MyProfileManager()
        {
            profileDisplay = new ObservableCollection<ProfileDisplay>()
            {
                new ProfileDisplay() {
                    TotalPurchasePrice = 0,
                    TotalEvaluatedPrice = 0,
                    EvaluationProfitAndLoss = 0,
                    ProfitRate = 0,
                    RealizedProfitAndLoss = 0,
                    TotalProfitAndLoss = 0,
                    SellAll = new RelayCommand((param)=>{
                        myPurchaseDisplay.ToList().FindAll(item => item.IsSelected).ForEach(item => {
                            SellItem(item.id);
                            myPurchaseDisplay.Remove(item);
                        }) ;
                    }),
                }
            };
            myPurchaseDisplay = new ObservableCollection<MyPurchaseDisplay>();
            myProfileInfos = new MyProfileInfos() { SeedMoney = 150000 };
        }

        /// <summary>
        /// 매입
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="purchasePrice"></param>
        /// <param name="purchaseAmount"></param>
        public void PurchaseItem(int itemId, float purchasePrice, int purchaseAmount)
        {
            float usageMoney = purchasePrice * purchaseAmount;
            if(myProfileInfos.SeedMoney < usageMoney)
            {
                MessageBox.Show("매입할 시드머니가 부족합니다.");
                return;
            }
            myProfileInfos.SeedMoney -= usageMoney;

            if(myProfileInfos.purchasedItemInfos.Any(item => item.id == itemId && item.purchasedPrice == purchasePrice))
            {
                myProfileInfos.purchasedItemInfos.Where(item => item.id == itemId && item.purchasedPrice == purchasePrice).First().purchasedAmount += purchaseAmount;
            }
            else
            {
                myProfileInfos.purchasedItemInfos.Add(new MyProfileInfos.PurchasedItemInfo()
                {
                    id = itemId,
                    purchasedPrice = purchasePrice,
                    purchasedAmount = purchaseAmount
                });
            }

            if (!myPurchaseDisplay.Any(item => item.id == itemId))
            {
                myPurchaseDisplay.Add(new MyPurchaseDisplay()
                {
                    id = itemId,
                    Quantity = purchaseAmount,
                    PurchaseAmount = purchasePrice * purchaseAmount,
                    StockName = StockManager.Instance.GetCompanyName(itemId),
                });
            }
            else
            {
                myPurchaseDisplay.Where(item => item.id == itemId).First().Quantity += purchaseAmount;
                myPurchaseDisplay.Where(item => item.id == itemId).First().PurchaseAmount += purchasePrice * purchaseAmount;

            }

            Update();
        }

        /// <summary>
        /// 아이템 매각
        /// </summary>
        /// <param name="id"></param>
        public void SellItem(int id)
        {
            List<MyProfileInfos.PurchasedItemInfo> sellList = myProfileInfos.purchasedItemInfos.Where(item => item.id == id).ToList();
            sellList.ForEach(item =>
            {
                myProfileInfos.SeedMoney += StockManager.Instance.GetCurrentPrice(item.id) * item.purchasedAmount;

            });
            foreach (var item in sellList)
            {
                myProfileInfos.purchasedItemInfos.Remove(item);
            }
        }

        public void Update()
        {
            float totalPurchasePrice = 0;
            float totalEvaluatedPrice = 0;

            myProfileInfos.purchasedItemInfos.ForEach(item =>
            {
                totalPurchasePrice += item.purchasedPrice * item.purchasedAmount;
                totalEvaluatedPrice += StockManager.Instance.GetCurrentPrice(item.id) * item.purchasedAmount;
                LogManager.Log($"amount: {item.purchasedAmount}");
            });

            profileDisplay.FirstOrDefault().TotalPurchasePrice = totalPurchasePrice;
            profileDisplay.FirstOrDefault().TotalEvaluatedPrice = totalEvaluatedPrice;

            myPurchaseDisplay.ToList().ForEach(item =>
            {
                item.CurrentPrice = StockManager.Instance.GetCurrentPrice(item.id);
            });

        }

    }
}
