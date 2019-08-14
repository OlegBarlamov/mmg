using System;
using FrameworkSDK.Configuration;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConfigureHandler : IAppConfigurator, IAppConfigureHandler
    {
        [NotNull]
        public PhaseConfiguration Configuration { get; set; } = new PhaseConfiguration();

        private bool _configured;

        private ModuleLogger Logger { get; set; }

        [NotNull] private IConfigurationProcessor ConfigurationProcessor { get; }

        public AppConfigureHandler([NotNull] IConfigurationProcessor configurationProcessor)
        {
            ConfigurationProcessor = configurationProcessor ?? throw new ArgumentNullException(nameof(configurationProcessor));
        }

        public void Configure()
        {
            ConfigureInternal();
        }

        public void Run()
        {
            ConfigureInternal();

            Logger = new ModuleLogger(FrameworkLogModule.Application);
            Logger.Info(Strings.Info.AppRunning);
        }

        public void Dispose()
        {

        }

        private void ConfigureInternal()
        {
            if (!_configured)
            {
                ConfigurationProcessor.Process(Configuration);
            }

            _configured = true;
        }
    }
}
