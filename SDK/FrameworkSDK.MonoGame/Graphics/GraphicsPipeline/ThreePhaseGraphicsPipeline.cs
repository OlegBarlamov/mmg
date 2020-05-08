using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Processing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public class ThreePhaseGraphicsPipeline : GraphicsPipelineBase
    {
        public override int ActionsCount => _processPipelineStep.Actions.Count;
        public override IGraphicsPipelineAction this[string actionName] => (IGraphicsPipelineAction)_processPipelineStep[actionName];
        protected override IGraphicsPipelineProcessor Processor { get; }
        
        private readonly PipelineStep _beginPipelineStep;
        private readonly PipelineStep _processPipelineStep;
        private readonly PipelineStep _endPipelineStep;

        public ThreePhaseGraphicsPipeline([NotNull] IGraphicsPipelineProcessor graphicsPipelineProcessor)
        {
            Processor = graphicsPipelineProcessor ?? throw new ArgumentNullException(nameof(graphicsPipelineProcessor));
            
            _beginPipelineStep = new PipelineStep("GraphicsBegin");
            _processPipelineStep = new PipelineStep("GraphicsProcess");
            _endPipelineStep = new PipelineStep("GraphicsEnd");
            
            Pipeline.Steps.Add(_beginPipelineStep);
            Pipeline.Steps.Add(_processPipelineStep);
            Pipeline.Steps.Add(_endPipelineStep);
        }
        
        public override void AddAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (Disposed) throw new ObjectDisposedException(nameof(ThreePhaseGraphicsPipeline));
            
            _processPipelineStep.AddAction(action);
        }

        public void AddBeginAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (Disposed) throw new ObjectDisposedException(nameof(ThreePhaseGraphicsPipeline));
            
            _beginPipelineStep.AddAction(action);
        }

        public void AddEndAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (Disposed) throw new ObjectDisposedException(nameof(ThreePhaseGraphicsPipeline));
            
            _endPipelineStep.AddAction(action);
        }
    }
}