using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegas.Client.MacOs
{
    public class GameResourcePackage : ResourcePackage
    {
        public Texture2D Circle { get; private set; }
        
        public SpriteFont Font { get; private set; } 
        
        protected override void Load(IContentLoaderApi content)
        {
            Circle = content.Primitives.Circle(100, Color.White);

            Font = content.Load<SpriteFont>("Font");
        }
    }
}