using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
    public interface IConfigurationProcessor
    {
        void Process([NotNull] PhaseConfiguration phaseConfiguration);
    }

    public sealed class SimpleConfigurationProcessor : ConfigurationProcessor, IConfigurationProcessor
    {
        protected override void OnPhaseActionProcessFailed(IConfigurationPhaseAction action, ConfigurationPhase phase, Exception error)
        {
            if (action.IsCritical)
                throw error;
        }

        protected override void OnPhaseProcessFailed(ConfigurationPhase phase, Exception error)
        {
            throw error;
        }
    }

    public abstract class ConfigurationProcessor : IConfigurationProcessor
    {
        /// <summary>
        /// Контекст конфигурирования. В нем можно передавать зависимости между фазами.
        /// Следует чистить на последнем шаге!
        /// </summary>
        //TODO назвать класс PipelineContext
        [NotNull] public NamedObjectsHeap Context { get; } = new NamedObjectsHeap();

        public void Process([NotNull] PhaseConfiguration phaseConfiguration)
        {
            if (phaseConfiguration == null) throw new ArgumentNullException(nameof(phaseConfiguration));
            var phases = phaseConfiguration.GetPhasesForProcess();

            OnPhasesReadyForProcess(phases);

            foreach (var phase in phases)
            {
                try
                {
                    OnPhaseProcessStarted(phase);

                    var actions = phase.GetActionsForProcess();

                    OnPhaseActionsReadyForProcess(actions, phase);

                    foreach (var action in actions)
                        ProcessPhaseAction(action, Context, phase);

                    OnPhaseProcessCompleted(phase);
                }
                catch (Exception e)
                {
                    OnPhaseProcessFailed(phase, e);
                }
            }

            OnConfigurationFinished();

            Context.Clear();
        }

        protected virtual void OnPhasesReadyForProcess(IReadOnlyList<ConfigurationPhase> phases)
        {

        }

        protected virtual void OnPhaseActionsReadyForProcess(IReadOnlyList<IConfigurationPhaseAction> actions, ConfigurationPhase phase)
        {

        }

        protected virtual void OnPhaseProcessStarted(ConfigurationPhase phase)
        {

        }

        protected virtual void OnPhaseProcessCompleted(ConfigurationPhase phase)
        {

        }

        protected virtual void OnPhaseProcessFailed(ConfigurationPhase phase, Exception error)
        {

        }

        protected virtual void OnPhaseActionProcessStarted(IConfigurationPhaseAction action, ConfigurationPhase phase)
        {

        }

        protected virtual void OnPhaseActionProcessFailed(IConfigurationPhaseAction action, ConfigurationPhase phase, Exception error)
        {

        }

        protected virtual void OnPhaseActionProcessCompleted(IConfigurationPhaseAction action, ConfigurationPhase phase)
        {

        }

        protected virtual void OnConfigurationFinished()
        {

        }

        private void ProcessPhaseAction(IConfigurationPhaseAction configurationPhaseAction, NamedObjectsHeap context, ConfigurationPhase phase)
        {
            try
            {
                OnPhaseActionProcessStarted(configurationPhaseAction, phase);
                configurationPhaseAction.Process(context);
                OnPhaseActionProcessCompleted(configurationPhaseAction, phase);
            }
            catch (Exception e)
            {
                OnPhaseActionProcessFailed(configurationPhaseAction, phase, e);
            }
        }
    }
}
