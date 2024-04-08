using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models
{
    public abstract class ViewModel
    {
        public virtual string GraphicsPassName { get; set; } = View.DefaultViewPassName;
    }

    public class ViewModel<T> : ViewModel
    {
        public T Model { get; }
            
        public ViewModel(T model)
        {
            Model = model;
        }
    }
}