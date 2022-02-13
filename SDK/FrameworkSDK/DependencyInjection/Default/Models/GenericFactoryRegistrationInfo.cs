using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    internal class GenericFactoryRegistrationInfo : IRegistrationInfo
    {
        public Type Type { get; }
        [NotNull] public ServiceFactoryDelegate Factory { get; }

        public ResolveType ResolveType { get; }

        private readonly object _disposableCollectingLock = new object();
        private readonly ConcurrentDictionary<int, object> _cashedInstances = new ConcurrentDictionary<int, object>();

        public GenericFactoryRegistrationInfo([NotNull] Type type, [NotNull] ServiceFactoryDelegate factory, ResolveType resolveType)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            CheckIfGeneralTypeDefinition(Type);
            ResolveType = resolveType;
        }

        private static void CheckIfGeneralTypeDefinition(Type type)
        {
            if (!type.IsGenericTypeDefinition) throw new ArgumentException("Type must be generic definition (Type<>) to use it for generic service registration. Actual type: " + type.Name);
        }

        public override string ToString()
        {
            return string.Format(NullFormatProvider.Instance, "{0}<>->factory():{1}[{2}({3})]", Type.Name, ResolveType, _cashedInstances.Count, GetCashedInstancesNames());
        }
        
        private string GetCashedInstancesNames()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return string.Join(",", _cashedInstances.Values.Select(instance => instance.GetTypeName()));
        }

        public object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters)
        {
            var genericArguments = type.GetGenericArguments();
            if (ResolveType == ResolveType.InstancePerResolve)
                return Factory(serviceLocator, type);
            
            lock (_disposableCollectingLock)
            {
                return _cashedInstances.GetOrAdd(genericArguments.GetHashCode(), hash => Factory(serviceLocator, type));
            }
        }

        public IEnumerable<IDisposable> GetDisposableCashedObjects()
        {
            lock (_disposableCollectingLock)
            {
                return _cashedInstances.Values
                    .Where(instance => instance is IDisposable)
                    .Cast<IDisposable>().ToArray();
            }
        }
    }
}