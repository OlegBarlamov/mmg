using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using FrameworkSDK.MonoGame.GameStructure.Views;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics
{
    public abstract class GraphicsPass : IGraphicsPass
    {
        public void Process(GameTime gameTime, IEnumerable<IView> views)
        {
            
        }

        public string Name { get; }
        public void Process(GameTime gameTime, IEnumerable<IGraphicComponent> views)
        {
            throw new System.NotImplementedException();
        }
    }
}
