using System;
using FrameworkSDK.Common;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Mapping;
using FrameworkSDK.Game.Mapping.Default;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Modules
{
	internal class CoreModule : IServicesModule
	{
		private IFrameworkLogger Logger { get; }
		private ILocalization Localization { get; }

		public CoreModule([NotNull] ILocalization localization, [NotNull] IFrameworkLogger logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Localization = localization ?? throw new ArgumentNullException(nameof(localization));
		}


		public void Register(IServiceRegistrator serviceRegistrator)
		{
			serviceRegistrator.RegisterInstance<ILocalization>(Localization);
			serviceRegistrator.RegisterInstance<IFrameworkLogger>(Logger);
			serviceRegistrator.RegisterInstance<IRandomService>(new DefaultRandomService(new Random(Guid.NewGuid().GetHashCode())));
		}
	}
}
