using System;
using FrameworkSDK.Configuration;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConfigureHandler : IAppConfigurator, IAppConfigureHandler
    {
        [NotNull]
        public PhaseConfiguration Configuration { get; set; } = new PhaseConfiguration();

        [NotNull] private IConfigurationProcessor ConfigurationProcessor { get; }

        public AppConfigureHandler([NotNull] IConfigurationProcessor configurationProcessor)
        {
            ConfigurationProcessor = configurationProcessor ?? throw new ArgumentNullException(nameof(configurationProcessor));
        }

        public void Run()
        {
            ConfigurationProcessor.Process(Configuration);
        }
    }
}
