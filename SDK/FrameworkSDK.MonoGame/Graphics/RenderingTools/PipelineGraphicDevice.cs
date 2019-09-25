using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    public class PipelineGraphicDevice : IGraphicDevice
    {
        private SpriteBatch SpriteBatch { get; }

        public PipelineGraphicDevice([NotNull] SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
        }

        public void BeginDraw(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            SpriteBatch.Begin(sortMode,blendState,samplerState,depthStencilState,rasterizerState,effect,transformMatrix);
        }

        public void EndDraw()
        {
            SpriteBatch.End();
        }
    }
}
