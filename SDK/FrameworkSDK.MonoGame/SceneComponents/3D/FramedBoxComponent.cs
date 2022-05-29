using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class BoxComponentDataModel
    {
        public Vector3 Position = Vector3.Zero;
        
        public Vector3 Origin = Vector3.Zero;
        public Vector3 Size = Vector3.One;
        
        public Color Color = Color.Red;

        public string GraphicsPassName = View.DefaultViewPassName;
    }
    
    public class FramedBoxComponent : SingleMeshComponent<BoxComponentDataModel>
    {
        protected override IRenderableMesh Mesh { get; }
        protected override string SingleGraphicsPassName { get; }

        public FramedBoxComponent(BoxComponentDataModel model)
        {
            SingleGraphicsPassName = model.GraphicsPassName;
            var mesh = new FixedSimpleMesh<VertexPositionColor>(this, PrimitiveType.LineList, VertexPositionColor.VertexDeclaration, new []
            {
                new VertexPositionColor(new Vector3(model.Origin.X - model.Size.X / 2, model.Origin.Y - model.Size.Y / 2, model.Origin.Z - model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X + model.Size.X / 2, model.Origin.Y - model.Size.Y / 2, model.Origin.Z - model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X - model.Size.X / 2, model.Origin.Y + model.Size.Y / 2, model.Origin.Z - model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X - model.Size.X / 2, model.Origin.Y - model.Size.Y / 2, model.Origin.Z + model.Size.Z / 2), model.Color),
                
                new VertexPositionColor(new Vector3(model.Origin.X + model.Size.X / 2, model.Origin.Y + model.Size.Y / 2, model.Origin.Z + model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X - model.Size.X / 2, model.Origin.Y + model.Size.Y / 2, model.Origin.Z + model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X + model.Size.X / 2, model.Origin.Y - model.Size.Y / 2, model.Origin.Z + model.Size.Z / 2), model.Color),
                new VertexPositionColor(new Vector3(model.Origin.X + model.Size.X / 2, model.Origin.Y + model.Size.Y / 2, model.Origin.Z - model.Size.Z / 2), model.Color),

            }, new []{ 0, 1, 0, 2, 0, 3, 1, 6, 1, 7, 2, 7, 2, 5, 3, 5, 3, 6, 4, 5, 4, 6, 4, 7}, 12);
            mesh.SetPosition(model.Position);

            Mesh = mesh;
        }
    }
}