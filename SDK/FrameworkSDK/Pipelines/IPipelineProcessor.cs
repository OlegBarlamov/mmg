using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
    public interface IPipelineProcessor
    {
        void Process([NotNull] Pipeline pipeline);
    }
}