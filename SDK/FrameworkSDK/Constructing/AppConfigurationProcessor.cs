using System;
using System.Collections.Generic;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConstructor : PipelineProcessor, IPipelineProcessor
    {
        [CanBeNull] private IFrameworkLogger ActiveLogger => _externalLogger ?? CustomLogger;
        [CanBeNull] private IFrameworkLogger CustomLogger => (IFrameworkLogger) Context.Heap[DefaultConfigurationSteps.ContextKeys.Logger];

        private ModuleLogger _moduleLogger;

        private readonly DeferredLogger _deferredLogger = new DeferredLogger();
        private readonly IFrameworkLogger _externalLogger;

        public AppConstructor([CanBeNull] IFrameworkLogger logger = null)
            : this(new PipelineContext(), logger)
        {
            _externalLogger = logger;
        }

        public AppConstructor([NotNull] PipelineContext pipelineContext, [CanBeNull] IFrameworkLogger logger = null)
            : base(pipelineContext)
        {
            _externalLogger = logger;
        }

        protected sealed override void OnStepsReadyForProcess(IReadOnlyList<PipelineStep> steps)
        {
            Log(Strings.Info.AppConstructing.ConstructingStart);

            LogConstructingMap(steps, FrameworkLogLevel.Debug);
        }

        protected sealed override void OnStepProcessStarted(PipelineStep step)
        {
            Log(string.Format(Strings.Info.AppConstructing.ConstructingStep, step));
        }

        protected sealed override void OnStepActionsReadyForProcess(IReadOnlyList<IPipelineAction> actions, PipelineStep step)
        {

        }

        protected sealed override void OnStepProcessCompleted(PipelineStep step)
        {

        }

        protected sealed override void OnStepProcessFailed(PipelineStep step, Exception error)
        {
            Log(string.Format(Strings.Exceptions.Constructing.StepFailed, step), FrameworkLogLevel.Error);
            throw error;
        }

        protected sealed override void OnStepActionProcessStarted(IPipelineAction action, PipelineStep step)
        {
            Log(string.Format(Strings.Info.AppConstructing.ConstructingAction, action));
        }

        protected sealed override void OnStepActionProcessFailed(IPipelineAction action, PipelineStep step, Exception error)
        {
            if (action.IsCritical)
                throw new AppConstructingException(Strings.Exceptions.Constructing.ActionFailed, error, action);

            Log(string.Format(Strings.Exceptions.Constructing.ActionFailed, action), FrameworkLogLevel.Error);
        }

        protected sealed override void OnStepActionProcessCompleted(IPipelineAction action, PipelineStep step)
        {

        }

        protected sealed override void OnConfigurationFinished()
        {
            Log(Strings.Info.AppConstructing.ConstructingEnd);

            _deferredLogger.Dispose();
            _moduleLogger.Dispose();
        }

        private void LogConstructingMap(IReadOnlyList<PipelineStep> steps, FrameworkLogLevel logLevel)
        {
            Log("Constructing map:", logLevel);

            foreach (var step in steps)
            {
                Log($" - {step}:", logLevel);
                foreach (var action in step.Actions)
                    Log($"      - {action}:", logLevel);
            }
        }

        private void Log(string message, FrameworkLogLevel logLevel = FrameworkLogLevel.Info)
        {
            if (_moduleLogger != null)
            {
                if (!_deferredLogger.IsEmpty)
                    _deferredLogger.LogTo(_moduleLogger);

                _moduleLogger.Log(message, logLevel);
                return;
            }

            if (ActiveLogger != null)
            {
                _moduleLogger = new ModuleLogger(ActiveLogger, FrameworkLogModule.Application);
                _deferredLogger.LogTo(_moduleLogger);
            }
            else
            {
                _deferredLogger.Log(message, FrameworkLogModule.Application, logLevel);
            }
        }
    }
}
