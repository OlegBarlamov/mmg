using System;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    internal class PipelineAction : IPipelineAction
    {
        public string Name { get; }
        public bool IsCritical { get; }

        private Action<IPipelineContext> ProcessAction { get; }

        public PipelineAction([NotNull] string name, bool isCritical, [NotNull] Action<IPipelineContext> processAction)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsCritical = isCritical;
            ProcessAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
        }

        public PipelineAction([NotNull] string name, [NotNull] Action<IPipelineContext> processAction)
            : this(name, false, processAction)
        {

        }

        public void Process(IPipelineContext context)
        {
            ProcessAction.Invoke(context);
        }

	    public override string ToString()
	    {
		    return Name;
	    }
    }
}
