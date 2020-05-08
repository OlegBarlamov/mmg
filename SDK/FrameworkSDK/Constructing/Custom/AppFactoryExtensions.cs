using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK
{
	public static class AppFactoryExtensions
	{
		public static IAppConfigurator Create<TApp>([NotNull] this AppFactory appFactory) where TApp : IApplication
		{
			if (appFactory == null) throw new ArgumentNullException(nameof(appFactory));

			var defaultConfigPipeline = appFactory.CreateDefaultPipeline();
			var configurator = new AppConfiguratorWithApplication<TApp>(appFactory.PipelineProcessor)
			{
				ConfigurationPipeline = defaultConfigPipeline
			};

			return configurator
				.RegisterServices(registrator => registrator.RegisterType<TApp, TApp>());
		}

        [NotNull]
	    public static Pipeline CreateDefaultPipeline(this AppFactory appFactory)
	    {
            var pipelineFactory = new DefaultConfigurationFactory();
	        return pipelineFactory.Create();
	    }
	}
}
