using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
	public class EmptyPipelineAction : IPipelineAction
	{
		public string Name { get; set; }
		public bool IsCritical { get; set; }

		public EmptyPipelineAction([NotNull] string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}
		
		public void Process(IPipelineContext context)
		{
		}
	}
}
