using FrameworkSDK.MonoGame.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace Console.FrameworkAdapter
{
    public interface IConsoleResourcePackage : IResourcePackage
    {
        Texture2D HeaderBackground { get; }
        Texture2D Background { get; }
        Texture2D CommandLineCorner { get; }
        Texture2D SuggestSelection { get; }
        SpriteFont ConsoleFont { get; }
    }
}