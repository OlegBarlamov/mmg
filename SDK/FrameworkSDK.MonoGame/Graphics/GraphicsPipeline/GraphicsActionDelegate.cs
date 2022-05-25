using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.GraphicsPipeline
{
    public delegate void GraphicsActionDelegate(GameTime gameTime, IGraphicDeviceContext graphicDeviceContext,
        IReadOnlyList<IGraphicComponent> components);

}