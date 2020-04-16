using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame
{
    public class InGameConsoleConfig
    {
        public float MinHeight { get; set; } = 100f;
        public float DefaultHeight { get; set; } = 320f;
        public float DefaultWidth { get; set; } = 640f;
        
        public Vector2 Position { get; set; } = Vector2.Zero;
        
        public Texture2D HeaderBackground { get; set; }
        
        public Texture2D Background { get; set; }

        public Texture2D CommandLineCorner { get; set; }

        public float CommandLineBoarderSize { get; set; } = 1f;
        
        public SpriteFont ConsoleFont { get; set; }

        public float ConsoleTextPadding { get; set; } = 10f;

        public float ConsoleCommandLineSectionHeight { get; set; } = 42f;

        public float ConsoleMessagesInterval { get; set; } = 2f;

        public TimeSpan AnimationTime { get; set; } = TimeSpan.FromMilliseconds(500);

        public float Opacity { get; set; } = 0.85f;

        public int CommandSuggestionsCount { get; set; } = 4;

        public Texture2D SuggestSelection { get; set; }
    }
}