
namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IGraphicComponent : IDrawableComponent, IRenderableComponent
    {
        string GraphicsPassName { get; }
    }
}
