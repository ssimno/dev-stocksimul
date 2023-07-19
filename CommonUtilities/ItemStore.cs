using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities
{
    public class ItemStore<T>
        where T : class
    {
        public event Action ChangedItem;
        public void OnChangedItem() => ChangedItem?.Invoke();
        private T _currentItem;
        public T CurrentItem
        {
            get => _currentItem;
            set
            {
                if (_currentItem == value) { return; }
                _currentItem = value;
                OnChangedItem();
            }
        }
    }
}
