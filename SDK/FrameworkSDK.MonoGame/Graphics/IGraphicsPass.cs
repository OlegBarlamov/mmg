using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IGraphicsPass : INamed
    {
        void Process(GameTime gameTime, IEnumerable<IGraphicComponent> views);
    }
}
