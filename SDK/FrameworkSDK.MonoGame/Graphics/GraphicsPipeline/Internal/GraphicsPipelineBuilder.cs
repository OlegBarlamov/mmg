using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    internal class GraphicsPipelineBuilder : IGraphicsPipelineBuilder
    {
        public IGraphicsPipeline Pipeline { get; }
        public IDrawContext DrawContext { get; }
        public IRenderContext RenderContext { get; }
        public IGraphicDeviceContext GraphicDeviceContext { get; }

        private bool _built;
        
        public IGraphicsPipelineBuilder AddAction([NotNull] IGraphicsPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_built) throw new FrameworkMonoGameException(Strings.Exceptions.Graphics.PipelineAlreadyBuilt);
            Pipeline.AddAction(action);
            return this;
        }

        public GraphicsPipelineBuilder(
            [NotNull] IGraphicsPipeline pipeline,
            [NotNull] IGraphicDeviceContext graphicDeviceContext,
            [NotNull] IDrawContext drawContext,
            [NotNull] IRenderContext renderContext)
        {
            Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            GraphicDeviceContext = graphicDeviceContext ?? throw new ArgumentNullException(nameof(graphicDeviceContext));
            DrawContext = drawContext ?? throw new ArgumentNullException(nameof(drawContext));
            RenderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }
        
        public IGraphicsPipeline Build()
        {
            if (_built) throw new FrameworkMonoGameException(Strings.Exceptions.Graphics.PipelineAlreadyBuilt);
            _built = true;
            return Pipeline;
        }
    }
}