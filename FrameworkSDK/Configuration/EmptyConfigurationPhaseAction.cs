using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
	public class EmptyConfigurationPhaseAction : IConfigurationPhaseAction
	{
		public string Name { get; set; }
		public bool IsCritical { get; set; }

		public EmptyConfigurationPhaseAction([NotNull] string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}
		
		public void Process(NamedObjectsHeap configureContext)
		{
		}
	}
}
