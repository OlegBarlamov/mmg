using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    public interface IRenderContext
    {
        IRenderer Renderer { get; }
    }
}
