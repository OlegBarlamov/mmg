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
        private bool _isDisposed;

        private ModuleLogger Logger { get; set; }

        [NotNull] private IPipelineProcessor PipelineProcessor { get; }

        public AppConfigureHandler([NotNull] IPipelineProcessor pipelineProcessor)
        {
            PipelineProcessor = pipelineProcessor ?? throw new ArgumentNullException(nameof(pipelineProcessor));
        }

        public void Configure()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AppConfigureHandler));
            ConfigureInternal();
        }

        public void Run()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AppConfigureHandler));
            ConfigureInternal();

            Logger = new ModuleLogger(FrameworkLogModule.Application);
            Logger.Info(Strings.Info.AppConstructing.AppRunning);
        }

        public void Dispose()
        {
            _isDisposed = true;
            Logger?.Dispose();
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
