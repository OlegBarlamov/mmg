using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Omegas.Client.MacOs
{
    public class MenuResourcePackage : ResourcePackage
    {
        public SpriteFont MenuFont { get; private set; }
        
        protected override void Load(IContentLoaderApi content)
        {
            MenuFont = content.Load<SpriteFont>("Font");
        }
    }
}