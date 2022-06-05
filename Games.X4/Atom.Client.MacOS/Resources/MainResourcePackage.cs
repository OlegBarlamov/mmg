using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS.Resources
{
    [UsedImplicitly]
    public class MainResourcePackage : ResourcePackage
    {
        public SpriteFont DebugInfoFont { get; private set; }
        public Texture2D StarTexture { get; private set; }
        public Texture2D GalaxyTexture { get; private set; }
        
        protected override void Load(IContentLoaderApi content)
        {
            StarTexture = content.Load<Texture2D>("star");
            GalaxyTexture = content.Load<Texture2D>("galaxy");
            DebugInfoFont = content.Load<SpriteFont>("debug_font");
        }
    }
}