using System;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Pipelines
{
    public class PipelineContext : IPipelineContext, IDisposable
    {
        [NotNull] public IKeyValueHeap<string, object> Heap { get; }

        public PipelineStep CurrentStep { get; set; }

        public PipelineContext()
            :this(new NamedObjectsHeap<object>())
        {
        }

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
