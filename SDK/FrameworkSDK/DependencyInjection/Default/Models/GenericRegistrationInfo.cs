using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
    internal class GenericRegistrationInfo : IRegistrationInfo
    {
        public Type Type { get; }

        public Type ImplType { get; }

        public ResolveType ResolveType { get; }

        private readonly object _disposableCollectingLock = new object();
        private readonly ConcurrentDictionary<int, object> _cashedInstances = new ConcurrentDictionary<int, object>();

        public GenericRegistrationInfo([NotNull] Type type, [NotNull] Type implType, ResolveType resolveType)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ImplType = implType ?? throw new ArgumentNullException(nameof(implType));
            CheckIfGeneralTypeDefinition(Type);
            CheckIfGeneralTypeDefinition(ImplType);
            ResolveType = resolveType;
        }

        private static void CheckIfGeneralTypeDefinition(Type type)
        {
            if (!type.IsGenericTypeDefinition) throw new ArgumentException("Type must be generic definition (Type<>) to use it for generic service registration. Actual type: " + type.Name);
        }

        public override string ToString()
        {
            return string.Format(NullFormatProvider.Instance, "{0}<>->{1}<>:{2}[{3}({4})]", Type.Name, ImplType.Name, ResolveType, _cashedInstances.Count, GetCashedInstancesNames());
        }

        private string GetCashedInstancesNames()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return string.Join(",", _cashedInstances.Values.Select(instance => instance.GetTypeName()));
        }

        public object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters)
        {
            var genericArguments = type.GetGenericArguments();
            var targetType = ImplType.MakeGenericType(genericArguments);
            if (ResolveType == ResolveType.InstancePerResolve)
                return serviceLocator.CreateInstance(targetType, parameters);
            
            lock (_disposableCollectingLock)
            {
                return _cashedInstances.GetOrAdd(genericArguments.GetHashCode(), hash => serviceLocator.CreateInstance(targetType, parameters));
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