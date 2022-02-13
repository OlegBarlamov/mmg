using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
	internal class TypeRegistrationInfo : IRegistrationInfo
	{
		public Type Type { get; }

		public Type ImplType { get; }

		public ResolveType ResolveType { get; }

		private readonly object _cashedInstanceLock = new object();
		[CanBeNull] private volatile object _cachedInstance;
		
		public TypeRegistrationInfo([NotNull] Type type, [NotNull] Type implType, ResolveType resolveType)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			ImplType = implType ?? throw new ArgumentNullException(nameof(implType));
			ResolveType = resolveType;
		}

		public override string ToString()
	    {
	        return string.Format(NullFormatProvider.Instance, "{0}->{1}:{2}[{3}]", Type.Name, ImplType.Name, ResolveType, _cachedInstance?.GetTypeName());
	    }

		public object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters)
		{
			if (ResolveType == ResolveType.InstancePerResolve)
				return serviceLocator.CreateInstance(ImplType, parameters);
				
			if (_cachedInstance != null)
				return _cachedInstance;
			
			lock (_cashedInstanceLock)
			{
				if (_cachedInstance != null)
					return _cachedInstance;

				_cachedInstance = serviceLocator.CreateInstance(ImplType, parameters);
			}
			return _cachedInstance;
		}


		public IEnumerable<IDisposable> GetDisposableCashedObjects()
		{
			if (_cachedInstance is IDisposable disposable)
				return new[] {disposable};
			return Array.Empty<IDisposable>();
		}
	}
}