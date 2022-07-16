using System;
using System.Collections.Concurrent;

namespace FrameworkSDK.Services.Default
{
    internal class DefaultDebugVariablesService : IDebugVariablesService, IDisposable
    {
        private readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();
        
        public T GetValue<T>(string key)
        {
            if (_dictionary.TryGetValue(key, out var value))
                return (T)value;

            return default;
        }

        public void SetValue<T>(string key, T value)
        {
            _dictionary.AddOrUpdate(key, value, (s, o) => value);
        }

        public void Dispose()
        {
            _dictionary.Clear();
        }
    }
}