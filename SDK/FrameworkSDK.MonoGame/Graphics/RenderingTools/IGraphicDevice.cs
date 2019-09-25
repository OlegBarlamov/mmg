using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.RenderingTools
{
    /// <summary>
    /// Graphics pipeline tool
    /// </summary>
    public interface IGraphicDevice
    {
        void BeginDraw(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);

        void EndDraw();
    }
}
