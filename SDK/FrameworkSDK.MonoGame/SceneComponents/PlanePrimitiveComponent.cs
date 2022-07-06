using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class PlanePrimitiveComponentData : SolidPrimitiveViewModel
    {
    }
    
    public class PlanePrimitiveComponent<TController> : RenderablePrimitive<PlanePrimitiveComponentData, TController> where TController : class, IController
    {
        public PlanePrimitiveComponent(PlanePrimitiveComponentData data) : base(new FixedSimpleMesh(StaticGeometries.Plane), data)
        {
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var meshGeometrySize = new Vector3(1, float.Epsilon, 1);
            return new BoundingBox(DataModel.Position - meshGeometrySize/2 * DataModel.Scale, DataModel.Position + meshGeometrySize/2 * DataModel.Scale);
        }
    }

    public class PlanePrimitiveComponent : PlanePrimitiveComponent<EmptyController>
    {
        public PlanePrimitiveComponent(PlanePrimitiveComponentData data) : base(data)
        {
        }
    }
}