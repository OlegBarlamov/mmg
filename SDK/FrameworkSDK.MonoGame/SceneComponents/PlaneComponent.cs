using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class PlaneComponentData : SolidPrimitiveViewModel
    {
    }
    
    public class PlaneComponent<TController> : RenderablePrimitive<PlaneComponentData, TController> where TController : IController
    {
        protected override BoundingBox MeshBoundingBox { get; }
        public PlaneComponent(PlaneComponentData data) : base(new FixedSimpleMesh(StaticGeometries.Plane), data)
        {
            var meshGeometrySize = new Vector3(1, float.Epsilon, 1);
            MeshBoundingBox = new BoundingBox(data.Position - meshGeometrySize/2 * data.Scale, data.Position + meshGeometrySize/2 * data.Scale);
        }
    }

    public class PlaneComponent : PlaneComponent<EmptyController>
    {
        public PlaneComponent(PlaneComponentData data) : base(data)
        {
        }
    }
}