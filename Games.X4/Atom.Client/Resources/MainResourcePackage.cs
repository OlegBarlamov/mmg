using System;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using NetExtensions.Helpers;

namespace Atom.Client.Resources
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

            var t = content.EmptyTexture(2, 2);
            t.SetDataToTexture(new[,]
            {
                {Color.Red, Color.Blue},
                {Color.Green, Color.Magenta},
            });
            A = t;

            Yellow = content.DiffuseColor(Color.Yellow);
        }
    }
}