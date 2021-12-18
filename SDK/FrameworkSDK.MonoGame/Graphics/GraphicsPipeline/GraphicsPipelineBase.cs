using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;

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

        private static IFrameworkLogger Logger { get; } = AppContext.Logger;

        protected GraphicsPipelineBase()
        {
            _internalPipelineProcessor = new InternalPipelineProcessor(
                ProcessActionFromInternalProcessor,
                new ModuleLogger(Logger, FrameworkLogModule.Rendering));
        }

        public virtual void Dispose()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(GraphicsPipelineBase));
            Disposed = true;

            Processor.Dispose();
            
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
            private ModuleLogger Logger { get; }

            public InternalPipelineProcessor(
                [NotNull] Action<IPipelineContext,IPipelineAction> processAction,
                [NotNull] ModuleLogger logger)
            {
                ProcessAction = processAction ?? throw new ArgumentNullException(nameof(processAction));
                Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            protected override void ProcessPipeLineAction(IPipelineContext context, IPipelineAction action)
            {
                ProcessAction(context, action);
            }
            
            protected override void OnStepActionProcessFailed(IPipelineAction action, PipelineStep step, Exception error)
            {
                if (action.IsCritical)
                    throw error;
                
                //TODO !!!
                Logger.Error("",error);
            }
        }
    }
}