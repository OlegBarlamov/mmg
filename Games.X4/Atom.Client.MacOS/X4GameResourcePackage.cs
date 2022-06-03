using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS
{
    public class X4GameResourcePackage : ResourcePackage
    {
        public Texture2D StarTexture { get; private set; }
        
        protected override void Load(IContentLoaderApi content)
        {
            StarTexture = content.Load<Texture2D>("star");
        }
    }
}