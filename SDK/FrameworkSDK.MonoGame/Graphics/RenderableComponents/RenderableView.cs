using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents
{
    public abstract class RenderableView<TData, TController> : View<TData, TController>  where TController : IController
    {
    }

    public abstract class RenderableView<TData> : RenderableView<TData, EmptyController>
    {
    }
}