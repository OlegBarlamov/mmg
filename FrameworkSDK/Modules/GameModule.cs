using System;
using FrameworkSDK.Common.Services.Graphics;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Mapping;
using FrameworkSDK.Game.Mapping.Default;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Modules
{
	internal class GameModule : IServicesModule
	{
		private GameShell GameShell { get; }
		private IFrameworkLogger Logger { get; }
		private IFrameworkServiceContainer MappingContainer { get; }

		public GameModule([NotNull] IFrameworkServiceContainer mappingContainer, [NotNull] IFrameworkLogger logger,
			[NotNull] GameShell gameShell)
		{
			GameShell = gameShell ?? throw new ArgumentNullException(nameof(gameShell));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			MappingContainer = mappingContainer ?? throw new ArgumentNullException(nameof(mappingContainer));
		}

		public void Register(IServiceRegistrator serviceRegistrator)
		{
			serviceRegistrator.RegisterInstance<IScenesController>(new ScenesController(Logger));
			var mappingHost = CreateMappingHost();
			serviceRegistrator.RegisterInstance<IViewResolver>(mappingHost);
			serviceRegistrator.RegisterInstance<IControllerResolver>(mappingHost);


			serviceRegistrator.RegisterInstance<ISpriteBatchProvider>(new DefaultSpriteBatchProvider(() => GameShell.SpriteBatch));
		}

		private MappingHost CreateMappingHost()
		{
			return new MappingHost(MappingContainer, AppDomain.CurrentDomain.GetAllTypes());
		}
	}
}
