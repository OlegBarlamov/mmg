using System;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Pipelines
{
    public class SimplePipelineProcessor : PipelineProcessor, IPipelineProcessor
    {
        protected override void OnStepActionProcessFailed(IPipelineAction action, PipelineStep step, Exception error)
        {
            if (action.IsCritical)
                throw error;
        }

        protected override void OnStepProcessFailed(PipelineStep step, Exception error)
        {
            throw error;
        }

        public SimplePipelineProcessor([NotNull] PipelineContext context)
            : base(context)
        {
        }

        public SimplePipelineProcessor()
            : base(PipelineContext.Empty)
        {
        }
    }
}