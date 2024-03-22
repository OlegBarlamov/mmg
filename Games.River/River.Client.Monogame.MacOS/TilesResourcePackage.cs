using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace River.Client.MacOS
{
    public class TilesResourcePackage : ResourcePackage
    {
        public Texture2D GroundTexture { get; private set; }
        public Texture2D WaterTexture { get; private set; }
        
        public Texture2D ArrowsTexture { get; private set; }

        protected override void Load(IContentLoaderApi content)
        {
            GroundTexture = content.DiffuseColor(Color.Brown);
            WaterTexture = content.DiffuseColor(Color.Blue);
            ArrowsTexture = content.DiffuseColor(Color.LightPink);
        }
    }
}