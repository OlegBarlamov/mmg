using FrameworkSDK.MonoGame.Graphics.Basic;
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
    }
    
    public class FramedBoxComponent : SingleMeshComponent<BoxComponentDataModel>
    {
        protected override BoundingBox BoundingBoxInternal { get; }
        protected override IRenderableMesh Mesh { get; }
        protected override string SingleGraphicsPassName { get; }

        public FramedBoxComponent(BoxComponentDataModel model)
        {
            SingleGraphicsPassName = model.GraphicsPassName;
            var mesh = new FixedSimpleMesh(this, new BoxGeometry(model.Color));
            mesh.SetPosition(model.Position);
            mesh.SetScale(model.Scale);
            
            Mesh = mesh;
            
            var meshGeometrySize = Vector3.One;
            BoundingBoxInternal = new BoundingBox((model.Position - meshGeometrySize/2) * model.Scale, (model.Position + meshGeometrySize/2) * model.Scale);
        }
    }
}