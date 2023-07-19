using CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using StockSimul.Scripts.Managers;
using StockSimul.UserControls.Stock;
using static StockSimul.Scripts.Managers.StockManager;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockSimul.Scripts.Command
{
    public class Command : Singleton<Command>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 쓰레드 상태
        /// </summary>
        public enum ThreadState
        {
            None = 0,
            Working
        }

        private ThreadState _prevGameState = ThreadState.None;  // 이전 쓰레드 상태
        private ThreadState _currentThreadState;                // 현재 쓰레드 상태
        public ThreadState CurrentThreadState
        {
            get => _currentThreadState;
            set
            {
                _prevGameState = _currentThreadState;
                _currentThreadState = value;
            }
        }

        private Dictionary<ThreadState, Action> stateHandleList { get; set; } // 상태 핸들 함수 배열

        private bool _isRunning = true;                                      // 쓰레드 bool
        private const float _tick = 2f;                                    // 쓰레드 틱
        private DateTime _currentDateTime;


        public DateTime CurrentDateTime {
            get => _currentDateTime;
            set { 
                _currentDateTime = value;
                OnPropertyChanged();
            } 
        }



        public Command() 
        {
            CurrentThreadState = ThreadState.None;


            stateHandleList = new Dictionary<ThreadState, Action>()
                {
                    {ThreadState.Working, Working},
                };
        }

        /// <summary>
        /// 쓰레드 시작
        /// </summary>
        private async void Run()
        {
            while (_isRunning)
            {
                if (_prevGameState != _currentThreadState)
                try
                {
                    if (stateHandleList.ContainsKey(CurrentThreadState))
                        stateHandleList[CurrentThreadState]();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                await Task.Delay((int)(_tick * 1000));
            }
        }

        /// <summary>
        /// 플레이 시작
        /// </summary>
        public virtual void StartPlay()
        {
            _isRunning = true;
            CurrentThreadState = ThreadState.Working;
            CurrentDateTime = new DateTime(2023, 1, 1, 9, 0, 0);


            
            Task.Run(Run);
        }


        /// <summary>
        /// 플레이 종료
        /// </summary>
        public virtual void StopPlay()
        {
            _isRunning = false;
        }

        public virtual void Working()
        {
            if (CurrentDateTime.Hour >= 16 || CurrentDateTime.DayOfWeek == DayOfWeek.Saturday || CurrentDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                do
                {
                    CurrentDateTime = CurrentDateTime.Date.AddDays(1);
                } while (CurrentDateTime.DayOfWeek == DayOfWeek.Saturday || CurrentDateTime.DayOfWeek == DayOfWeek.Sunday);

                CurrentDateTime = CurrentDateTime.AddHours(9);
            }
            else
                CurrentDateTime = CurrentDateTime.AddHours(1);


            LogManager.Log($"Date: {CurrentDateTime.ToString("yyyy년MM월dd일")}");
            LogManager.Log($"Time: {CurrentDateTime.ToString("HH:mm:ss")}");
            LogManager.Log($"");

            Application.Current.Dispatcher?.BeginInvoke((Action)(() =>
            {
                StockManager.Instance.StockItems.ToList().ForEach(item => {
                    item.StockInfo.Update();
                    LogManager.Log($"Name: {item.StockInfo.CompanyName}, Price: {item.StockInfo.Price}, Gap: {item.StockInfo.Price - item.StockInfo.PrevPrice}, PrevPrice: {item.StockInfo.PrevPrice}");

                });

                StockItemInfo selectedItem = StockManager.Instance.StockItems.ToList().Where(
                    item => item.StockInfo.Id == StockManager.Instance.detailPanel.selectedId).FirstOrDefault()?.StockInfo;
                StockManager.Instance.detailPanel.Update(selectedItem);
                MyProfileManager.Instance.Update();
            }));

            

            //StockManager.Instance.itemList.ForEach(item => {
            //    item.Update();
            //    LogManager.Log($"Name: {item.Name}, Price: {item.Price}, Gap: {item.Price-item.PrevPrice}, PrevPrice: {item.PrevPrice}");
            //});


            LogManager.Log($"=================================================");
        }
       
    }
}
