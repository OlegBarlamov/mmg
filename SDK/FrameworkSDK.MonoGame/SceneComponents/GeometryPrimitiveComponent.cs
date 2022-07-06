using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class GeometryPrimitiveComponentData : SolidPrimitiveViewModel
    {
        public readonly IMeshGeometry MeshGeometry;

        public GeometryPrimitiveComponentData(IMeshGeometry meshGeometry)
        {
            MeshGeometry = meshGeometry;
        }
    }
    
    public class GeometryPrimitiveComponent<TController> : RenderablePrimitive<GeometryPrimitiveComponentData, TController> where TController : class, IController
    {
        public GeometryPrimitiveComponent(GeometryPrimitiveComponentData data) : base(CreateMesh(data), data)
        {
        }

        private static FixedSimpleMesh CreateMesh(GeometryPrimitiveComponentData data)
        {
            return new FixedSimpleMesh(data.MeshGeometry);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var meshGeometrySize = new Vector3(1, 1, 1);
            return new BoundingBox(DataModel.Position - meshGeometrySize/2 * DataModel.Scale, DataModel.Position + meshGeometrySize/2 * DataModel.Scale);
        }
    }

    public class GeometryPrimitiveComponent : GeometryPrimitiveComponent<EmptyController>
    {
        public GeometryPrimitiveComponent(GeometryPrimitiveComponentData data) : base(data)
        {
        }
    }
}