
namespace FrameworkSDK.Game
{
    public interface IGraphicComponent : IDrawableComponent, IRenderableComponent
    {
        string GraphicsPassName { get; }
    }
}
