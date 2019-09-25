using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    public interface IGraphicsPipelineContext : IDisposable
    {
        IGraphicDevice GraphicDevice { get; }
        IRenderContext RenderContext { get; }
        IDrawContext DrawContext { get; }
    }

    public class GraphicsPipelineContext : IGraphicsPipelineContext
    {
        public IGraphicDevice GraphicDevice =>
            _graphicDevice ?? (_graphicDevice = new PipelineGraphicDevice(GameHeart.SpriteBatch));
        public IRenderContext RenderContext =>
            _renderContext ?? (_renderContext = new RenderContext());
        public IDrawContext DrawContext =>
            _drawContext ?? (_drawContext = new DrawContext());

        private IGraphicDevice _graphicDevice;
        private IRenderContext _renderContext;
        private IDrawContext _drawContext;

        private IGameHeart GameHeart { get; }

        public GraphicsPipelineContext([NotNull] IGameHeart gameHeart)
        {
            GameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));
        }

        public void Dispose()
        {
            
        }
    }
}
