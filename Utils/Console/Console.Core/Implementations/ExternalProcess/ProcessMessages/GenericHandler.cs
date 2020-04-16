using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Console.Core.Implementations.ExternalProcess.ProcessMessages
{
    internal class GenericHandler<T> where T : class
    {
        private readonly Dictionary<int, Action<T>> _handlers = new Dictionary<int, Action<T>>();
        
        public void Add<TInstance>([NotNull] Action<TInstance> handler) where TInstance : T
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            
            var type = typeof(TInstance);
            var hash = type.GetHashCode();
            _handlers.Add(hash, (x) => handler((TInstance)x));
        }

        public void Handle([NotNull] T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            var type = instance.GetType();
            var hash = type.GetHashCode();

            var handler = _handlers[hash];
            handler.Invoke(instance);
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}