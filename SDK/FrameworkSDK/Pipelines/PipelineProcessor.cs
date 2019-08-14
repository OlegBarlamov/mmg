using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
    public abstract class PipelineProcessor : IPipelineProcessor
    {
        protected PipelineContext Context { get; }

        protected PipelineProcessor([NotNull] PipelineContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Process([NotNull] Pipeline pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
            var steps = pipeline.GetStepsForProcess();

            OnPhasesReadyForProcess(steps);

            foreach (var step in steps)
            {
                try
                {
                    Context.CurrentStep = step;
                    OnPhaseProcessStarted(step);

                    var actions = step.GetActionsForProcess();

                    OnPhaseActionsReadyForProcess(actions, step);

                    foreach (var action in actions)
                        ProcessPhaseAction(action, Context, step);

                    OnPhaseProcessCompleted(step);
                }
                catch (Exception e)
                {
                    OnPhaseProcessFailed(step, e);
                }
            }

            Context.CurrentStep = null;
            OnConfigurationFinished();

            Context.Dispose();
        }

        protected virtual void OnPhasesReadyForProcess(IReadOnlyList<PipelineStep> phases)
        {

        }

        protected virtual void OnPhaseActionsReadyForProcess(IReadOnlyList<IPipelineAction> actions, PipelineStep phase)
        {

        }

        protected virtual void OnPhaseProcessStarted(PipelineStep phase)
        {

        }

        protected virtual void OnPhaseProcessCompleted(PipelineStep phase)
        {

        }

        protected virtual void OnPhaseProcessFailed(PipelineStep phase, Exception error)
        {

        }

        protected virtual void OnPhaseActionProcessStarted(IPipelineAction action, PipelineStep phase)
        {

        }

        protected virtual void OnPhaseActionProcessFailed(IPipelineAction action, PipelineStep phase, Exception error)
        {

        }

        protected virtual void OnPhaseActionProcessCompleted(IPipelineAction action, PipelineStep phase)
        {

        }

        protected virtual void OnConfigurationFinished()
        {

        }

        private void ProcessPhaseAction(IPipelineAction pipelineAction, IPipelineContext context, PipelineStep phase)
        {
            try
            {
                OnPhaseActionProcessStarted(pipelineAction, phase);
                pipelineAction.Process(context);
                OnPhaseActionProcessCompleted(pipelineAction, phase);
            }
            catch (Exception e)
            {
                OnPhaseActionProcessFailed(pipelineAction, phase, e);
            }
        }
    }
}
