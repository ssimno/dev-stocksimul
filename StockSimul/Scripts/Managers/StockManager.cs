using CommonUtilities;
using StockSimul.UserControls.Stock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static StockSimul.Scripts.Managers.StockManager;

namespace StockSimul.Scripts.Managers
{
    public class StockManager : Singleton<StockManager>
    {
        public class StockItemInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


            /// <summary>
            /// 이벤트 타입
            /// </summary>
            public enum EventType
            {
                None = 0,
                Stable,         // 변동 없음 또는 매우 사소한 변동
                SlightRise,     // 약간의 가격 상승
                ModerateRise,   // 보통 정도의 가격 상승
                StrongRise,     // 큰 폭의 가격 상승
                RapidRise,      // 급격한 가격 상승
                SlightFall,     // 약간의 가격 하락
                ModerateFall,   // 보통 정도의 가격 하락
                StrongFall,     // 큰 폭의 가격 하락
                RapidFall,      // 급격한 가격 하락
                Unchanged       // 가격 변동 없음
            }


            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string IndustryType { get; set; }



            private float price;
            public float Price
            {
                get { return price; }
                set
                {
                    // set price
                    price = value > 0 ? value : 0;

                    string arrowMark = string.Empty;
                    string ratio = string.Empty;

                    if (Price - PrevPrice > 0)
                    {
                        ViewFontColor = new SolidColorBrush(Color.FromRgb(255, 105, 97)); 
                        arrowMark = "▲";
                    }
                    else if (Price - PrevPrice < 0)
                    {
                        ViewFontColor = new SolidColorBrush(Color.FromRgb(100, 149, 237)); 
                        arrowMark = "▼";
                    }
                    else
                    {
                        ViewFontColor = new SolidColorBrush(Color.FromRgb(211, 211, 211)); 
                        arrowMark = "";
                    }



                    ratio = (((Price - PrevPrice) / PrevPrice) * 100).ToString("N2");

                    // set change price
                    ViewPriceChange = $"{Math.Truncate(Price - PrevPrice).ToString()} {arrowMark}  {ratio}";

                    ViewCurrentPrice = Math.Truncate(value).ToString();
                    OnPropertyChanged(nameof(Price));
                }
            }

            public float PrevPrice { get; set; } = 0;
            public float ChangePrice { get; set; } = 0;


            
            private string viewCurrentPrice = string.Empty;
            /// <summary>
            /// 주식아이템 시각화 - 현재 주가
            /// </summary>
            public string ViewCurrentPrice {
                get 
                { 
                    return viewCurrentPrice;
                } 
                set 
                {

                    viewCurrentPrice = UtilityScript.InsertComma(value);
                    OnPropertyChanged(nameof(ViewCurrentPrice));
                } 
            }

            private string viewPriceChange = string.Empty;
            /// <summary>
            /// 주식아이템 시각화 - 전 주가와 차이
            /// </summary>
            public string ViewPriceChange
            {
                get
                {
                    return viewPriceChange;
                }
                set
                {

                    viewPriceChange = value;
                    OnPropertyChanged(nameof(ViewPriceChange));
                }
            }


            private Brush viewFontColor = Brushes.Black;
            /// <summary>
            /// 주식아이템 시각화 - 주가 등락에 따른 색 변경
            /// </summary>
            public Brush ViewFontColor
            {
                get {
                    return viewFontColor;
                }
                set {
                    viewFontColor = value;
                    OnPropertyChanged(nameof(ViewFontColor));
                }
            }


            private Brush viewSelectedColor;
            /// <summary>
            /// 주식아이템 시각화 - 선택된 아이템 색 변경
            /// </summary>
            public Brush ViewSelectedColor
            {
                get
                {
                    return viewSelectedColor;
                }
                set
                {
                    viewSelectedColor = value;
                    OnPropertyChanged(nameof(ViewSelectedColor));
                }
            }

            private Visibility delistingVisible;
            /// <summary>
            /// 상장폐지 여부
            /// </summary>
            public Visibility DelistingVisible
            {
                get
                {
                    return delistingVisible;
                }
                set
                {
                    delistingVisible = value;
                    OnPropertyChanged(nameof(delistingVisible));
                }
            }

            

            /// <summary>
            /// 주식아이템 시각화 - 아이템 선택
            /// </summary>
            public ICommand MouseUpCommand { get; set; }

            public event Action<StockItemInfo> ItemSelected;


            private Random _rnd;
            private List<float> _changeList;


            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="id"></param>
            /// <param name="companyName"></param>
            /// <param name="industryType"></param>
            /// <param name="price"></param>
            public StockItemInfo(int id, string companyName, string industryType, float price)
            {
                (this.Id, this.CompanyName, this.IndustryType, this.Price) = (id, companyName, industryType, price);

                this._rnd = new Random((int)DateTime.Now.Ticks + id);
                _changeList =  new List<float>();
                
                // 아이템 클릭
                MouseUpCommand = new RelayCommand((param) => {
                    ItemSelected?.Invoke(this);
                });
            }

            /// <summary>
            /// 데이터 갱신
            /// </summary>
            public void Update()
            {
                PrevPrice = Price;

                if (_changeList.Count == 0)
                {
                    int steps = _rnd.Next(1, 10);

                    AddEvent(PickEvent(), steps);
                }

                Price += _changeList[0];
                _changeList.RemoveAt(0);
            }

            /// <summary>
            /// 주가 이벤트 추가
            /// </summary>
            /// <param name="type"></param>
            /// <param name="changeStep"></param>
            public void AddEvent(EventType type, int changeStep)
            {
                _changeList.Clear();

                float totalChange;

                switch (type)
                {
                    case EventType.Unchanged:
                        totalChange = 0f;
                        break;
                    case EventType.Stable:
                        totalChange = Price * ((float)_rnd.NextDouble() * 0.1f - 0.05f) * (float)Math.Sqrt(_rnd.NextDouble());
                        break;
                    case EventType.SlightRise:
                        totalChange = Price * (float)_rnd.NextDouble() * 0.05f;
                        break;
                    case EventType.ModerateRise:
                        totalChange = Price * (float)_rnd.NextDouble() * 0.1f + 0.05f;
                        break;
                    case EventType.StrongRise:
                        totalChange = Price * (float)_rnd.NextDouble() * 0.15f + 0.1f;
                        break;
                    case EventType.RapidRise:
                        totalChange = Price * (float)_rnd.NextDouble() * 1.5f + 0.4f;
                        break;
                    case EventType.SlightFall:
                        totalChange = -Price * (float)_rnd.NextDouble() * 0.05f;
                        break;
                    case EventType.ModerateFall:
                        totalChange = -Price * (float)_rnd.NextDouble() * 0.1f + 0.05f;
                        break;
                    case EventType.StrongFall:
                        totalChange = -Price * (float)_rnd.NextDouble() * 0.15f + 0.1f;
                        break;
                    case EventType.RapidFall:
                        totalChange = -Price * (float)_rnd.NextDouble() * 1.5f + 0.4f;
                        break;
                    default:
                        totalChange = Price * ((float)_rnd.NextDouble() * 0.6f - 0.3f);
                        break;
                }

                int steps = _rnd.Next(1, changeStep);

                float remainingChange = totalChange;
                for (int i = 0; i < steps; i++)
                {
                    float changeThisStep = (float)_rnd.NextDouble() * remainingChange;
                    _changeList.Add(changeThisStep);
                    remainingChange -= changeThisStep;
                }
                _changeList[steps - 1] += remainingChange;
            }

            /// <summary>
            /// 이벤트 선택
            /// </summary>
            /// <returns></returns>
            private EventType PickEvent()
            {
                Dictionary<EventType, int> probability = new Dictionary<EventType, int>()
                {
                    { EventType.Stable, 25},
                    { EventType.SlightRise, 5},
                    { EventType.ModerateRise, 3},
                    { EventType.StrongRise, 2},
                    { EventType.RapidRise, 1},
                    { EventType.SlightFall, 5},
                    { EventType.ModerateFall, 3},
                    { EventType.StrongFall, 2},
                    { EventType.RapidFall, 1},
                    { EventType.Unchanged, 10},
                };
                List<EventType> events = probability.SelectMany(pair => Enumerable.Repeat(pair.Key, pair.Value)).ToList();

                return events[_rnd.Next(events.Count)];
            }

        }


        // 주식 종목 아이템 UserControl 리스트
        public ObservableCollection<StockItem> StockItems { get; set; }

        public class DetailPanel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public int selectedId;

            private string companyName;
            public string CompanyName {
                get => companyName;
                set {
                    companyName = value;
                    OnPropertyChanged();
                }
            }

            private float currentPrice;
            public float CurrentPrice
            {
                get => currentPrice;
                set
                {
                    currentPrice = value;
                    OnPropertyChanged();
                }
            }

            private float totalPrice;
            public float TotalPrice
            {
                get => totalPrice;
                set
                {
                    totalPrice = value;
                    OnPropertyChanged();
                }
            }

            private int priceCount;
            public int PriceCount
            {
                get => priceCount;
                set
                {
                    priceCount = value;

                    ChangeTotalPrice();
                    OnPropertyChanged();
                }
            }


            public DetailPanel()
            {
                selectedId = 0;
                PriceCount = 1;
                currentPrice= 0;
                TotalPrice = 0;
            }

            public void Update(StockItemInfo info)
            {
                if (info == null)
                    return;

                CurrentPrice = info.Price;
                CompanyName = info.CompanyName;

                ChangeTotalPrice();
            }

            public void ChangeTotalPrice()
            {
                TotalPrice = CurrentPrice * PriceCount;
            }
        }
        public DetailPanel detailPanel { get; }
        public ICommand PurchaseCommand { get; set; }

        public StockManager() 
        {
            detailPanel = new DetailPanel();
            StockItems = new ObservableCollection<StockItem>();

            List<StockItemInfo> itemList = new List<StockItemInfo>()
            {
                new StockItemInfo(1, "삼성전자", "전자제품 제조", 1000),
                new StockItemInfo(2, "SK하이닉스", "반도체 제조", 2000),
                new StockItemInfo(3, "LG화학", "화학제품 제조", 12000),
                new StockItemInfo(4, "현대차", "자동차 제조", 500),
                new StockItemInfo(5, "셀트리온", "바이오 제약", 300),
                new StockItemInfo(6, "네이버", "인터넷 서비스", 54000),
                new StockItemInfo(7, "카카오", "인터넷 서비스", 130000),
                new StockItemInfo(8, "삼성SDI", "전지 제조", 12000),
                new StockItemInfo(9, "한화솔루션", "화학제품 제조", 90000),
                new StockItemInfo(10, "아모레퍼시픽", "화장품 제조", 200000),
                new StockItemInfo(11, "LG생활건강", "화장품 제조", 50000),
                new StockItemInfo(12, "POSCO", "철강 제조", 32000),
                new StockItemInfo(13, "KT&G", "담배 제조", 15000),
                new StockItemInfo(14, "현대모비스", "자동차부품 제조", 60500),
                new StockItemInfo(15, "LG전자", "전자제품 제조", 10300)
            };

            itemList.ForEach(item =>
            {
                item.ItemSelected += SelectedItem;
                StockItems.Add(new StockItem(item));
            });

            PurchaseCommand = new RelayCommand((param) => {
                MyProfileManager.Instance.PurchaseItem(
                    itemId: detailPanel.selectedId,
                    purchasePrice: detailPanel.CurrentPrice, 
                    purchaseAmount: detailPanel.PriceCount);
            });


            // 기본 선택
            SelectedItem(StockItems.ToList().Where(item => item.StockInfo.Id == 1).FirstOrDefault()?.StockInfo);
        }


        /// <summary>
        /// 아이템 선택
        /// </summary>
        /// <param name="selectedItem"></param>
        public void SelectedItem(StockItemInfo selectedItem)
        {
            if (selectedItem == null)
                return;

            foreach (var nonSelectedItem in StockItems)
            {
                nonSelectedItem.StockInfo.ViewSelectedColor = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            }
            selectedItem.ViewSelectedColor = Brushes.Yellow;

            detailPanel.PriceCount = 1;
            detailPanel.selectedId = selectedItem.Id;
            detailPanel.Update(selectedItem);
        }

        /// <summary>
        /// 현재 가격
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float GetCurrentPrice(int id) => StockItems.ToList().Where(item => item.StockInfo.Id == id).FirstOrDefault()?.StockInfo.Price ?? 0;

        /// <summary>
        /// 종목 이름
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetCompanyName(int id) => StockItems.ToList().Where(item => item.StockInfo.Id == id).FirstOrDefault()?.StockInfo.CompanyName ?? "";
    }
}
