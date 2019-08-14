using System;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Pipelines
{
    public interface IPipelineContext
    {
        PipelineStep CurrentStep { get; }

        [NotNull] IKeyValueHeap<string, object> Heap { get; }
    }

    public class PipelineContext : IPipelineContext, IDisposable
    {
        [NotNull] public IKeyValueHeap<string, object> Heap { get; }

        public PipelineStep CurrentStep { get; set; }

        public PipelineContext(IKeyValueHeap<string, object> objectsHeap)
        {
            Heap = objectsHeap;
        }

        public void Dispose()
        {
            Heap.Clear();
            CurrentStep = null;
        }
    }
}
