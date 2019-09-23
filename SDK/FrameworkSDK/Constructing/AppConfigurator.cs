using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    public class AppConfigurator : IAppConfigurator, IAppRunner
    {
        [NotNull]
        public Pipeline ConfigurationPipeline { get; set; } = new Pipeline();

        private bool _configured;
        private bool _isDisposed;

        private ModuleLogger Logger { get; set; }

        [NotNull] private IPipelineProcessor PipelineProcessor { get; }

        public AppConfigurator([NotNull] IPipelineProcessor pipelineProcessor)
        {
            PipelineProcessor = pipelineProcessor ?? throw new ArgumentNullException(nameof(pipelineProcessor));
        }

        public virtual IAppRunner Configure()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AppConfigurator));
            ConfigureInternal();

	        return this;
        }

        public virtual void Run()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AppConfigurator));
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
