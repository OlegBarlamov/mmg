using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public abstract class GraphicsPipelineBase : IGraphicsPipeline
    {
        public abstract int ActionsCount { get; }
        public abstract IGraphicsPipelineAction this[string actionName] { get; }
        
        protected abstract IGraphicsPipelineProcessor Processor { get; }
        
        protected bool Disposed { get; private set; }
        protected Pipeline Pipeline { get; } = new Pipeline();

        private GameTime _currentTime;

        private readonly InternalPipelineProcessor _internalPipelineProcessor;

        protected GraphicsPipelineBase()
        {
            _internalPipelineProcessor = new InternalPipelineProcessor(ProcessActionFromInternalProcessor);
        }

        public virtual void Dispose()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(GraphicsPipelineBase));
            Disposed = true;

            var steps = Pipeline.GetStepsForProcess();
            foreach (var step in steps)
            {
                var actions = step.GetActionsForProcess();
                foreach (var action in actions)
                {
                    if (action is IDisposable disposable)
                    {
                        using (disposable)
                        {
                            //dispose
                        }
                    }
                }
            }
        }
        
        public abstract void AddAction(IGraphicsPipelineAction action);
        
        public void SetupComponents(IReadOnlyObservableList<IGraphicComponent> componentsCollection)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(GraphicsPipelineBase));
            
            Processor.SetupComponents(componentsCollection);
        }

        public void Process(GameTime gameTime)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(GraphicsPipelineBase));
            _currentTime = gameTime;
            
            _internalPipelineProcessor.Context.Heap.Clear();
            _internalPipelineProcessor.Process(Pipeline);
        }

        private void ProcessActionFromInternalProcessor(IPipelineContext context, IPipelineAction action)
        {
            var graphicPipelineAction = action as IGraphicsPipelineAction;
            if (graphicPipelineAction == null) throw new Exception(); //TODO
            
            Processor.Process(_currentTime, context, graphicPipelineAction);
        }

        private class InternalPipelineProcessor : SimplePipelineProcessor
        {
            private Action<IPipelineContext, IPipelineAction> ProcessAction { get; }

            public InternalPipelineProcessor([NotNull] Action<IPipelineContext,IPipelineAction> processAction)
            {
                ProcessAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
            }

            protected override void ProcessPipeLineAction(IPipelineContext context, IPipelineAction action)
            {
                base.ProcessPipeLineAction(context, action);

                ProcessAction(context, action);
            }
        }
    }
}