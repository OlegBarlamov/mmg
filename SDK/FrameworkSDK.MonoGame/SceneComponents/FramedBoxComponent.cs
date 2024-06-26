using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class FramedBoxComponentDataModel : ViewModel3D
    {
        public Color Color = Color.Red;

        public static FramedBoxComponentDataModel FromBoundingBox(BoundingBox boundingBox)
        {
            return new FramedBoxComponentDataModel
            {
                Position = (boundingBox.Max + boundingBox.Min) / 2,
                Scale = boundingBox.Max - boundingBox.Min
            };
        }
    }
    
    public class FramedBoxComponent : RenderablePrimitive<FramedBoxComponentDataModel>
    {
        public FramedBoxComponent(FramedBoxComponentDataModel viewModel) : base(CreateMesh(viewModel.Color), viewModel)
        {
        }

        private static IRenderableMesh CreateMesh(Color color)
        {
            return new FixedSimpleMesh(new ColoredFramedBoxGeometry(color));
        }

        public void SetName(string name)
        {
            Name = name;
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            var meshGeometrySize = Vector3.One;
            return new BoundingBox(DataModel.Position - meshGeometrySize/2 * DataModel.Scale, DataModel.Position + meshGeometrySize/2 * DataModel.Scale);
        }
    }
}