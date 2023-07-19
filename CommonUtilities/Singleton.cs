using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

        public static T Instance => lazyInstance.Value;

        protected Singleton()
        {
            // Protected constructor to prevent external instantiation
        }

    }
}
