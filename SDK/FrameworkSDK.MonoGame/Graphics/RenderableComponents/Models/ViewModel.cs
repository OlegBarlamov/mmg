using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models
{
    public abstract class ViewModel
    {
        public virtual string GraphicsPassName { get; set; } = View.DefaultViewPassName;
    }
}