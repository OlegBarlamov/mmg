using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC.Default
{
	internal class RegistrationInfo : IDisposable
	{
		[NotNull] public Type Type { get; }

		[NotNull] public Type ImplType { get; }

		public ResolveType ResolveType { get; }

		[CanBeNull] public object CashedInstance { get; set; }

		private RegistrationInfo([NotNull] Type type, [NotNull] Type implType, ResolveType resolveType)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			ImplType = implType ?? throw new ArgumentNullException(nameof(implType));
			ResolveType = resolveType;
		}

		[NotNull]
		public static RegistrationInfo FromInstance([NotNull] Type serviceType, [NotNull] object instance)
		{
		    if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
		    if (instance == null) throw new ArgumentNullException(nameof(instance));

			var type = instance.GetType();
			return new RegistrationInfo(serviceType, type, ResolveType.Singletone)
			{
				CashedInstance = instance
			};
		}

	    public override string ToString()
	    {
	        return string.Format(NullFormatProvider.Instance, "{0}->{1}:{2}", Type.Name, ImplType.Name, ResolveType);
	    }

		[NotNull]
		public static RegistrationInfo FromType([NotNull] Type serviceType, [NotNull] Type implType, ResolveType resolveType)
		{
			if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
			if (implType == null) throw new ArgumentNullException(nameof(implType));

			return new RegistrationInfo(serviceType, implType, resolveType);
		}

		public void Dispose()
		{
			CashedInstance = null;
		}
	}
}