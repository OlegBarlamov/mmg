using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.FrameworkAdapter
{
    public sealed class DefaultResourcePackage : ResourcePackage, IConsoleResourcePackage
    {
        public Texture2D HeaderBackground { get; private set; } 
        public Texture2D Background { get; private set; }
        public Texture2D CommandLineCorner { get; private set; }
        public Texture2D SuggestSelection { get; private set; }
        public SpriteFont ConsoleFont { get; private set; }

        private DefaultParameters Parameters { get; }
        
        public DefaultResourcePackage()
            :this(null)
        {
        }
        
        public DefaultResourcePackage(DefaultParameters parameters = null)
        {
            Parameters = parameters ?? new DefaultParameters();
        }
        
        protected override void Load(IContentLoaderApi content)
        {
            HeaderBackground = content.GradientColor(Parameters.HeaderAmbientColor, Parameters.BackgroundColor, 30, 20, 90, 0.8f);
            Background = content.DiffuseColor(Parameters.BackgroundColor);
            CommandLineCorner = content.DiffuseColor(Parameters.CommandLineColor);
            SuggestSelection = content.DiffuseColor(Parameters.SuggestSelectionColor);
            ConsoleFont = content.Load<SpriteFont>(Parameters.FontAssetName);
        }

        public class DefaultParameters
        {
            public Color BackgroundColor { get; set; } = new Color(31,32,36);
            public Color HeaderAmbientColor { get; set; } = new Color(26, 54, 84);
            public Color CommandLineColor { get; set; } = Color.White;
            public Color SuggestSelectionColor { get; set; } = Color.Orange;
            public string FontAssetName { get; set; } = "TextFont";
        }
    }
}