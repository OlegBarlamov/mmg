using System;

namespace FrameworkSDK.Common
{
    public class Singletone<T> where T : class
    {
        private T _instance;
        
        private readonly object _locker = new object();
        
        public T GetOrCreate(Func<T> factory)
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = factory();
                }
            }

            return _instance;
        }

        public T GetOrEmpty()
        {
            return _instance;
        }
    }
}