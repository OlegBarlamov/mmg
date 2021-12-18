using System;
using JetBrains.Annotations;
using NetExtensions;
using NetExtensions.Collections;

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
        
        public static PipelineContext Empty => new PipelineContext(new NamedObjectsHeap<object>());

        public void Dispose()
        {
            Heap.Clear();
            CurrentStep = null;
        }
    }
}
