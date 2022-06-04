using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS.Resources
{
    [UsedImplicitly]
    public class LoadingSceneResources : ResourcePackage
    {
        public SpriteFont Font { get; private set; }
        public Texture2D LoadingSceneBackgroundTexture { get; private set; }

        protected override void Load(IContentLoaderApi content)
        {
            LoadingSceneBackgroundTexture = content.GradientColor(Color.Black, Color.LightGray, 1, 120, 90);
            Font = content.Load<SpriteFont>("font");
        }
    }
}