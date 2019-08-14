using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
    //TODO rename to pipeline
    public class ConfigurationPhase : INamed
    {
        public string Name { get; }

        public ConfigurationPhase([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull, ItemNotNull] public IReadOnlyList<IConfigurationPhaseAction> Actions => _actions;

        private readonly List<IConfigurationPhaseAction> _actions = new List<IConfigurationPhaseAction>();

        [NotNull]
        public IConfigurationPhaseAction this[[NotNull] string actionName]
        {
            get
            {
                if (actionName == null) throw new ArgumentNullException(nameof(actionName));
                if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentException(nameof(actionName));

                if (!ContainsWithName(actionName))
                    throw new ConfigurationException();

                return _actions.First(action => action.Name == actionName);
            }
        }

        public void AddAction([NotNull] IConfigurationPhaseAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (ContainsWithName(action.Name))
                throw new ConfigurationException();

            _actions.Add(action);
        }

        public void RemoveAction([NotNull] IConfigurationPhaseAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!ContainsWithName(action.Name))
                throw new ConfigurationException();

            _actions.Remove(action);
        }

        public bool ContainsAction([NotNull] IConfigurationPhaseAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return _actions.Contains(action);
        }

        public virtual IReadOnlyList<IConfigurationPhaseAction> GetActionsForProcess()
        {
            return Actions.ToArray();
        }

        private bool ContainsWithName(string actionName)
        {
            return _actions.ContainsWithName(actionName);
        }
    }
}