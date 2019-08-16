using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Pipelines
{
    public interface IPipelineContext
    {
        PipelineStep CurrentStep { get; }

        [NotNull] IKeyValueHeap<string, object> Heap { get; }
    }
}