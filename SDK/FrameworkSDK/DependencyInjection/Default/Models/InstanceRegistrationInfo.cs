using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.DependencyInjection.Default.Models
{
	internal class InstanceRegistrationInfo : IRegistrationInfo
	{
		public Type Type { get; }

		public Type ImplType { get; }
		public ResolveType ResolveType { get; } = ResolveType.Singletone;

		[NotNull] public object SingletoneInstance { get; }
		
		public InstanceRegistrationInfo([NotNull] Type type, [NotNull] object implementationInstance)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			SingletoneInstance = implementationInstance ?? throw new ArgumentNullException(nameof(implementationInstance));
			ImplType = SingletoneInstance.GetType();
		}

		public override string ToString()
	    {
	        return string.Format(NullFormatProvider.Instance, "{0}->[{1}]", Type.Name, SingletoneInstance.GetTypeName());
	    }

		public object GetOrSet(Type type, IServiceLocator serviceLocator, object[] parameters)
		{
			return SingletoneInstance;
		}

		public IEnumerable<IDisposable> GetDisposableCashedObjects()
		{
			if (SingletoneInstance is IDisposable disposable)
				return new[] {disposable};
			return Array.Empty<IDisposable>();
		}
	}
}