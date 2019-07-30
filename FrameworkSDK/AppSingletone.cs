using System;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK
{
	internal static class AppSingletone
	{
		public static IFrameworkLogger Logger { get; private set; }
		public static IServiceLocator ServiceLocator { get; private set; }

		internal static void Initialize([NotNull] IFrameworkLogger logger, [NotNull] IServiceLocator serviceLocator)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
		}
	}
}
