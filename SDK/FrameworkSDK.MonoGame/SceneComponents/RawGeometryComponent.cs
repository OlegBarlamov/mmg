using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class RawGeometryComponentData : SolidPrimitiveViewModel
    {
        public readonly IMeshGeometry MeshGeometry;

        public RawGeometryComponentData(IMeshGeometry meshGeometry)
        {
            MeshGeometry = meshGeometry;
        }
    }
    
    public class RawGeometryComponent<TController> : RenderablePrimitive<RawGeometryComponentData, TController> where TController : IController
    {
        protected override BoundingBox MeshBoundingBox { get; }
        public RawGeometryComponent(RawGeometryComponentData data) : base(CreateMesh(data), data)
        {
            var meshGeometrySize = new Vector3(1, 1, 1);
            MeshBoundingBox = new BoundingBox(data.Position - meshGeometrySize/2 * data.Scale, data.Position + meshGeometrySize/2 * data.Scale);
        }

        private static FixedSimpleMesh CreateMesh(RawGeometryComponentData data)
        {
            return new FixedSimpleMesh(data.MeshGeometry);
        }
    }

    public class RawGeometryComponent : RawGeometryComponent<EmptyController>
    {
        public RawGeometryComponent(RawGeometryComponentData data) : base(data)
        {
        }
    }
}