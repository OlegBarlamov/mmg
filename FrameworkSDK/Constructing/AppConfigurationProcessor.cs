using System;
using System.Collections.Generic;
using FrameworkSDK.Configuration;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class AppConfigurationProcessor : ConfigurationProcessor, IConfigurationProcessor
    {
        private IFrameworkLogger Logger => (IFrameworkLogger)(Context["logger"] ?? _defaultLogger);

        private readonly IFrameworkLogger _defaultLogger;

        public AppConfigurationProcessor([CanBeNull] IFrameworkLogger logger = null)
        {
            _defaultLogger = logger ?? new NullLogger();
        }

        protected sealed override void OnPhasesReadyForProcess(IReadOnlyList<ConfigurationPhase> phases)
        {

        }

        protected sealed override void OnPhaseActionsReadyForProcess(IReadOnlyList<IConfigurationPhaseAction> actions, ConfigurationPhase phase)
        {

        }

        protected sealed override void OnPhaseProcessStarted(ConfigurationPhase phase)
        {

        }

        protected sealed override void OnPhaseProcessCompleted(ConfigurationPhase phase)
        {

        }

        protected sealed override void OnPhaseProcessFailed(ConfigurationPhase phase, Exception error)
        {

        }

        protected sealed override void OnPhaseActionProcessStarted(IConfigurationPhaseAction action, ConfigurationPhase phase)
        {

        }

        protected sealed override void OnPhaseActionProcessFailed(IConfigurationPhaseAction action, ConfigurationPhase phase, Exception error)
        {

        }

        protected sealed override void OnPhaseActionProcessCompleted(IConfigurationPhaseAction action, ConfigurationPhase phase)
        {

        }

        protected sealed override void OnConfigurationFinished()
        {
            //Нужно провермить Context на пустату. Если что кинуть warning!
            throw new NotImplementedException();
        }
    }
}
