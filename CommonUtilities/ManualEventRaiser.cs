using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities
{
    /// <summary>
    /// last edit: 2022-11-08
    /// 
    /// 조건 없이 수동으로 이벤트를 발생시키는 클래스,
    /// 다양한 클래스에서 동시다발적으로 이벤트 발생시킬 때 사용
    /// </summary>
    public class ManualEventRaiser
    {
        #region Singletone
        // 싱글턴과 일반 인스턴스 사용을 겸함
        private static ManualEventRaiser _instance = null;
        public static ManualEventRaiser Instance => _instance ?? (_instance = new ManualEventRaiser());
        #endregion
        public event Action ManualEvent;
        public void OnManualEvent() => ManualEvent?.Invoke();
    }
}
