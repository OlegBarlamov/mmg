using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK
{
	public static class AppFactoryExtensions
	{
		public static IAppConfigurator Create<TApp>([NotNull] this AppFactory appFactory) where TApp : IApplication
		{
			if (appFactory == null) throw new ArgumentNullException(nameof(appFactory));

			var defaultConfigPipeline = new DefaultConfigurationFactory().Create();
			var configurator = new AppConfiguratorWithApplication<TApp>(appFactory.PipelineProcessor)
			{
				ConfigurationPipeline = defaultConfigPipeline
			};

			return configurator
				.RegisterServices(registrator => registrator.RegisterType<TApp, TApp>());
		}
	}

	internal class AppConfiguratorWithApplication<TApp> : AppConfigurator where TApp : IApplication
	{
		public AppConfiguratorWithApplication([NotNull] IPipelineProcessor pipelineProcessor)
			: base(pipelineProcessor)
		{
		}

		public override void Run()
		{
			var app = CreateApp();

			base.Run();

			app.Run();
		}

		private static TApp CreateApp()
		{
			if (AppContext.ServiceLocator != null)
				return AppContext.ServiceLocator.Resolve<TApp>();

			return Activator.CreateInstance<TApp>();
		}
	}

	public interface IApplication
	{
		void Run();
	}
}
