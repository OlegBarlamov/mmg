using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame.Graphics.DrawableComponents
{
    public abstract class DrawablePrimitive<TData, TController> : View<TData, TController> where TData : ViewModel where TController : IController
    {
        protected DrawablePrimitive(TData data)
        {
            GraphicsPassNames = new List<string>
            {
                data.GraphicsPassName
            };
        }

        public override IReadOnlyList<string> GraphicsPassNames { get; }
    }

    public abstract class DrawablePrimitive<TData> : DrawablePrimitive<TData, EmptyController> where TData : ViewModel
    {
        protected DrawablePrimitive(TData data) : base(data)
        {
        }
    }
}