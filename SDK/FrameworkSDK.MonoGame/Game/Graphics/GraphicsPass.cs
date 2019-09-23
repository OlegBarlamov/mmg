using System.Collections.Generic;
using FrameworkSDK.Game.Views;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Graphics
{
    public abstract class GraphicsPass : IGraphicsPass
    {
        public void Process(GameTime gameTime, IEnumerable<IView> views)
        {
            
        }
    }
}
