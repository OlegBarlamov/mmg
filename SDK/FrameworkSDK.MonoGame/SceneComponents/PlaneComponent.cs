using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class PlaneComponentData
    {
        public Vector3 Position = Vector3.Zero;
        
        public Vector3 Scale = Vector3.One;

        public string GraphicsPassName = View.DefaultViewPassName;

        public IMeshMaterial Material;

        public PlaneComponentData(IMeshMaterial material)
        {
            Material = material;
        }
    }
    
    public class PlaneComponent : SingleMeshComponent<PlaneComponentData>
    {
        protected override BoundingBox BoundingBoxInternal { get; }
        protected override IRenderableMesh Mesh { get; }
        protected override string SingleGraphicsPassName { get; }

        public PlaneComponent(PlaneComponentData data)
        {
            SingleGraphicsPassName = data.GraphicsPassName;
            
            Mesh = new FixedSimpleMesh(this, StaticGeometries.Plane)
            {
                Position = data.Position,
                Scale = data.Scale,
                Material = data.Material,
            };
            
            var meshGeometrySize = new Vector3(1, float.Epsilon, 1);
            BoundingBoxInternal = new BoundingBox(data.Position - meshGeometrySize/2 * data.Scale, data.Position + meshGeometrySize/2 * data.Scale);
        }
    }
}