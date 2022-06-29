using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class RawGeometryComponentData
    {
        public Vector3 Position = Vector3.Zero;
        
        public Vector3 Scale = Vector3.One;

        public string GraphicsPassName = View.DefaultViewPassName;

        public IMeshMaterial Material;

        public IMeshGeometry MeshGeometry;

        public RawGeometryComponentData(IMeshMaterial material, IMeshGeometry meshGeometry)
        {
            Material = material;
            MeshGeometry = meshGeometry;
        }
    }
    
    public class RawGeometryComponent : SingleMeshComponent<RawGeometryComponentData>
    {
        protected override BoundingBox BoundingBoxInternal { get; }
        protected override IRenderableMesh Mesh { get; }
        protected override string SingleGraphicsPassName { get; }

        public RawGeometryComponent(RawGeometryComponentData data)
        {
            SingleGraphicsPassName = data.GraphicsPassName;
            
            Mesh = new FixedSimpleMesh(this, data.MeshGeometry)
            {
                Position = data.Position,
                Scale = data.Scale,
                Material = data.Material,
            };
            
            var meshGeometrySize = new Vector3(1, 1, 1);
            BoundingBoxInternal = new BoundingBox(data.Position - meshGeometrySize/2 * data.Scale, data.Position + meshGeometrySize/2 * data.Scale);
        }
    }
}