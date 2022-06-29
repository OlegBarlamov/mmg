using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class BoxComponentDataModel
    {
        public Vector3 Position = Vector3.Zero;
        
        public Vector3 Scale = Vector3.One;
        
        public Color Color = Color.Red;

        public string GraphicsPassName = View.DefaultViewPassName;

        public static BoxComponentDataModel FromBoundingBox(BoundingBox boundingBox)
        {
            return new BoxComponentDataModel
            {
                Position = (boundingBox.Max + boundingBox.Min) / 2,
                Scale = boundingBox.Max - boundingBox.Min
            };
        }
    }
    
    public class FramedBoxComponent : SingleMeshComponent<BoxComponentDataModel>
    {
        protected override BoundingBox BoundingBoxInternal { get; }
        protected override IRenderableMesh Mesh { get; }
        protected override string SingleGraphicsPassName { get; }

        public FramedBoxComponent(BoxComponentDataModel model)
        {
            SingleGraphicsPassName = model.GraphicsPassName;
            var mesh = new FixedSimpleMesh(this, new ColoredFramedBoxGeometry(model.Color));
            mesh.SetPosition(model.Position);
            mesh.SetScale(model.Scale);
            
            Mesh = mesh;
            
            var meshGeometrySize = Vector3.One;
            BoundingBoxInternal = new BoundingBox(model.Position - meshGeometrySize/2 * model.Scale, model.Position + meshGeometrySize/2 * model.Scale);
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}