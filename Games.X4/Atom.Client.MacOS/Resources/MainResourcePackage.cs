using System;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Helpers;

namespace Atom.Client.MacOS.Resources
{
    [UsedImplicitly]
    public class MainResourcePackage : ResourcePackage
    {
        public SpriteFont DebugInfoFont { get; private set; }
        public Texture2D StarTexture { get; private set; }
        public Texture2D GalaxyTexture { get; private set; }
        
        public Texture2D A { get; private set; }
        
        public Texture2D Yellow { get; private set; }
        
        protected override void Load(IContentLoaderApi content)
        {
            StarTexture = content.Load<Texture2D>("star");
            GalaxyTexture = content.Load<Texture2D>("galaxy");
            DebugInfoFont = content.Load<SpriteFont>("debug_font");

            var array = ArrayGenerator.GetRandomArray(0, 1, 10, 10, new Random());
            A = content.HeightMap(array, 0, 1, Color.White, Color.Blue);

            Yellow = content.DiffuseColor(Color.Yellow);
        }
    }
}