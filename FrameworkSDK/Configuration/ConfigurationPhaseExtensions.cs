using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
	public static class ConfigurationPhaseExtensions
	{
		public static void AddActions([NotNull] this ConfigurationPhase phase, [NotNull] IEnumerable<IConfigurationPhaseAction> actions)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actions == null) throw new ArgumentNullException(nameof(actions));

			foreach (var action in actions)
				phase.AddAction(action);
		}

		public static void AddActions(this ConfigurationPhase phase, params IConfigurationPhaseAction[] actions)
		{
			AddActions(phase, (IEnumerable<IConfigurationPhaseAction>)actions);
		}

		public static void AddOrReplace([NotNull] this ConfigurationPhase phase, [NotNull] IConfigurationPhaseAction action)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (action == null) throw new ArgumentNullException(nameof(action));

			if (phase.ContainsName(action.Name))
				phase.RemoveByName(action.Name);

			phase.AddAction(action);
		}

		public static bool ContainsName([NotNull] this ConfigurationPhase phase, [NotNull] string actionName)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actionName == null) throw new ArgumentNullException(nameof(actionName));
			return phase.Actions.ContainsWithName(actionName);
		}

		public static void RemoveByName([NotNull] this ConfigurationPhase phase, [NotNull] string actionName)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actionName == null) throw new ArgumentNullException(nameof(actionName));

			var targetAction = phase.Actions.FindByName(actionName);
			phase.RemoveAction(targetAction);
		}
	}
}
