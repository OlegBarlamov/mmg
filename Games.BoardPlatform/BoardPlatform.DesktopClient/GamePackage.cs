using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework;

namespace TablePlatform.DesktopClient
{
    public class GamePackage : ResourcePackage
    {
        public TextureUnifiedWrapper Texture { get; private set; }
        
        protected override void Load(IContentLoaderApi content)
        {
            var texture = content.GradientColor(Color.Blue, Color.Red, 30, 20, 90, 0.8f);
            Texture = new TextureUnifiedWrapper(texture);
        }
    }
}