using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
	public class EmptyPipelineAction : IPipelineAction
	{
		public string Name { get; }
	    public bool IsCritical { get; } = false;

		public EmptyPipelineAction([NotNull] string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}
		
		public void Process(IPipelineContext context)
		{
		}
	}
}
