using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.FrameworkAdapter
{
    public static class AppConfiguratorExtensions
    {
        public static IAppConfigurator SetupCustomLoggerIfNotNull([NotNull] this IAppConfigurator configurator,
            [CanBeNull] IFrameworkLogger frameworkLogger)
        {
            if (configurator == null) throw new ArgumentNullException(nameof(configurator));
            if (frameworkLogger != null)
            {
                configurator.SetupCustomLogger(frameworkLogger);
            }
        }

        public static IAspNetCoreAppConfigurator UseAspNetCore([NotNull] this IAppConfigurator appConfigurator, [NotNull] IHostBuilder hostBuilder)
        {
            if (appConfigurator == null) throw new ArgumentNullException(nameof(appConfigurator));
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));

            var pipelineStep = new PipelineStep("AspNetCore");
            pipelineStep.AddAction(new SimplePipelineAction("Running", true, context => RunAspNetCore(hostBuilder)));
            appConfigurator.ConfigurationPipeline.Steps.Add(pipelineStep);
            return new HostBuilderAppConfiguratorCombination(hostBuilder, appConfigurator);
        }

        private static void RunAspNetCore([NotNull] IHostBuilder hostBuilder)
        {
            if (hostBuilder == null) throw new ArgumentNullException(nameof(hostBuilder));
            hostBuilder.Build().Run();
        }
    }
    

}