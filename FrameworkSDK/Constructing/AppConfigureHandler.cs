using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConfigureHandler : IAppConfigurator, IAppConfigureHandler
    {
        [NotNull]
        public Pipeline ConfigurationPipeline { get; set; } = new Pipeline();

        private bool _configured;

        private ModuleLogger Logger { get; set; }

        [NotNull] private IPipelineProcessor PipelineProcessor { get; }

        public AppConfigureHandler([NotNull] IPipelineProcessor pipelineProcessor)
        {
            PipelineProcessor = pipelineProcessor ?? throw new ArgumentNullException(nameof(pipelineProcessor));
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
                PipelineProcessor.Process(ConfigurationPipeline);
            }

            _configured = true;
        }
    }
}
