using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.DrawableComponents
{
    public abstract class DrawablePrimitive<TData, TController> : View<TData, TController> where TData : ViewModel where TController : class, IController
    {
        protected DrawablePrimitive(TData data)
        {
            GraphicsPassNames = new List<string>
            {
                data.GraphicsPassName
            };
            SetDataModel(data);
        }

        public override IReadOnlyList<string> GraphicsPassNames { get; }
        
        public void AssignControllerToPrimitive(TController controller)
        {
            if (Controller != null)
                throw new FrameworkMonoGameException($"Drawable primitive {Name} already has controller assigned: {Controller}");
            
            SetController(controller);
            controller.SetDataModel(DataModel);
            controller.SetView(this);
        }
    }

    public abstract class DrawablePrimitive<TData> : DrawablePrimitive<TData, EmptyController> where TData : ViewModel
    {
        protected DrawablePrimitive(TData data) : base(data)
        {
        }
    }
}