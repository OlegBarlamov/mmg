using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    internal class GraphicsPipelineContextFactory : IGraphicsPipelineContextFactory
    {
        private IGameHeart GameHeart { get; }

        public GraphicsPipelineContextFactory([NotNull] IGameHeart gameHeart)
        {
            GameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));
        }

        public IGraphicsPipelineContext Create()
        {
            return new GraphicsPipelineContext(GameHeart);
        }
    }
}
