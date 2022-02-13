using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    public class FactoryRegistrationInfo : IRegistrationInfo
    {
        public Type Type { get; }
        public ServiceFactoryDelegate Factory { get; }
        public ResolveType ResolveType { get; }
        
        private readonly object _cashedInstanceLock = new object();
        private volatile object _cachedInstance;

        public FactoryRegistrationInfo([NotNull] Type type, [NotNull] ServiceFactoryDelegate factory, ResolveType resolveType)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            ResolveType = resolveType;
        }
        
        public override string ToString()
        {
            return string.Format(NullFormatProvider.Instance, "{0}->factory():{1}[{2}]", Type.Name, ResolveType, _cachedInstance?.GetTypeName());
        }
        
        public object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters)
        {
            if (ResolveType == ResolveType.InstancePerResolve)
                return Factory(serviceLocator, type);
            
            if (_cachedInstance != null)
                return _cachedInstance;
			
            lock (_cashedInstanceLock)
            {
                if (_cachedInstance != null)
                    return _cachedInstance;

                _cachedInstance = Factory(serviceLocator, type);
            }
            return _cachedInstance;
        }

        public IEnumerable<IDisposable> GetDisposableCashedObjects()
        {
            return Array.Empty<IDisposable>();
        }
    }
}