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

            OnStepsReadyForProcess(steps);

            foreach (var step in steps)
            {
                try
                {
                    Context.CurrentStep = step;
                    OnStepProcessStarted(step);

                    var actions = step.GetActionsForProcess();

                    OnStepActionsReadyForProcess(actions, step);

                    foreach (var action in actions)
                        ProcessPhaseAction(action, Context, step);

                    OnStepProcessCompleted(step);
                }
                catch (Exception e)
                {
                    OnStepProcessFailed(step, e);
                }
            }

            Context.CurrentStep = null;
            OnConfigurationFinished();

            Context.Dispose();
        }

        protected virtual void OnStepsReadyForProcess(IReadOnlyList<PipelineStep> steps)
        {

        }

        protected virtual void OnStepActionsReadyForProcess(IReadOnlyList<IPipelineAction> actions, PipelineStep step)
        {

        }

        protected virtual void OnStepProcessStarted(PipelineStep step)
        {

        }

        protected virtual void OnStepProcessCompleted(PipelineStep step)
        {

        }

        protected virtual void OnStepProcessFailed(PipelineStep step, Exception error)
        {

        }

        protected virtual void OnStepActionProcessStarted(IPipelineAction action, PipelineStep step)
        {

        }

        protected virtual void OnStepActionProcessFailed(IPipelineAction action, PipelineStep step, Exception error)
        {

        }

        protected virtual void OnStepActionProcessCompleted(IPipelineAction action, PipelineStep step)
        {

        }

        protected virtual void OnConfigurationFinished()
        {

        }

        private void ProcessPhaseAction(IPipelineAction pipelineAction, IPipelineContext context, PipelineStep phase)
        {
            try
            {
                OnStepActionProcessStarted(pipelineAction, phase);
                pipelineAction.Process(context);
                OnStepActionProcessCompleted(pipelineAction, phase);
            }
            catch (Exception e)
            {
                OnStepActionProcessFailed(pipelineAction, phase, e);
            }
        }
    }
}
