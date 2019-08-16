using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
	public static class PipelineStepExtensions
	{
		public static void AddActions([NotNull] this PipelineStep phase, [NotNull] IEnumerable<IPipelineAction> actions)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actions == null) throw new ArgumentNullException(nameof(actions));

			foreach (var action in actions)
				phase.AddAction(action);
		}

		public static void AddActions(this PipelineStep phase, params IPipelineAction[] actions)
		{
			AddActions(phase, (IEnumerable<IPipelineAction>)actions);
		}

		public static void AddOrReplace([NotNull] this PipelineStep phase, [NotNull] IPipelineAction action)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (action == null) throw new ArgumentNullException(nameof(action));

			if (phase.ContainsName(action.Name))
				phase.RemoveByName(action.Name);

			phase.AddAction(action);
		}

		public static bool ContainsName([NotNull] this PipelineStep phase, [NotNull] string actionName)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actionName == null) throw new ArgumentNullException(nameof(actionName));
			return phase.Actions.ContainsByName(actionName);
		}

		public static void RemoveByName([NotNull] this PipelineStep phase, [NotNull] string actionName)
		{
			if (phase == null) throw new ArgumentNullException(nameof(phase));
			if (actionName == null) throw new ArgumentNullException(nameof(actionName));

			var targetAction = phase.Actions.FindByName(actionName);
			phase.RemoveAction(targetAction);
		}
	}
}
