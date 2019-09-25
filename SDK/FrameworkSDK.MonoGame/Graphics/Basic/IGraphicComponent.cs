
namespace FrameworkSDK.MonoGame.Graphics.Basic
{
    public interface IGraphicComponent : IDrawableComponent, IRenderableComponent
    {
        string GraphicsPassName { get; }
    }
}
