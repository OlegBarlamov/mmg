using System;
using System.Collections.Generic;
using FrameworkSDK.Logging;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Constructing
{
    internal class AppConstructor : PipelineProcessor, IPipelineProcessor
    {
        private IFrameworkLogger Logger => (IFrameworkLogger)(Context.Heap[DefaultConfigurationSteps.ContextKeys.Logger] ?? _defaultLogger);

        private readonly IFrameworkLogger _defaultLogger;

        public AppConstructor([CanBeNull] IFrameworkLogger logger = null) : this(
            new PipelineContext(new NamedObjectsHeap<object>()), logger)
        {
            _defaultLogger = logger ?? new NullLogger();
        }

        public AppConstructor([NotNull] PipelineContext pipelineContext, [CanBeNull] IFrameworkLogger logger = null) : base(
            pipelineContext)
        {
            _defaultLogger = logger ?? new NullLogger();
        }

        protected sealed override void OnPhasesReadyForProcess(IReadOnlyList<PipelineStep> phases)
        {

        }

        protected sealed override void OnPhaseActionsReadyForProcess(IReadOnlyList<IPipelineAction> actions, PipelineStep phase)
        {

        }

        protected sealed override void OnPhaseProcessStarted(PipelineStep phase)
        {

        }

        protected sealed override void OnPhaseProcessCompleted(PipelineStep phase)
        {

        }

        protected sealed override void OnPhaseProcessFailed(PipelineStep phase, Exception error)
        {

        }

        protected sealed override void OnPhaseActionProcessStarted(IPipelineAction action, PipelineStep phase)
        {

        }

        protected sealed override void OnPhaseActionProcessFailed(IPipelineAction action, PipelineStep phase, Exception error)
        {

        }

        protected sealed override void OnPhaseActionProcessCompleted(IPipelineAction action, PipelineStep phase)
        {

        }

        protected sealed override void OnConfigurationFinished()
        {
        }
    }
}
