using JetBrains.Annotations;
using NetExtensions;
using NetExtensions.Collections;

namespace FrameworkSDK.Pipelines
{
    public interface IPipelineContext
    {
        PipelineStep CurrentStep { get; }

        [NotNull] IKeyValueHeap<string, object> Heap { get; }
    }
}