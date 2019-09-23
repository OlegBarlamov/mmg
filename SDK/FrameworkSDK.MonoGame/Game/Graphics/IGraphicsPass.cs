using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Graphics
{
    public interface IGraphicsPass : INamed
    {
        void Process(GameTime gameTime, IEnumerable<IGraphicComponent> views);
    }
}
