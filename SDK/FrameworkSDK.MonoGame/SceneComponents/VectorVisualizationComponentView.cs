using System;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class VectorVisualizationComponentDataModel : ViewModel3D
    {
        private static readonly Color DefaultColor = Color.LightBlue; 
        
        public Color Color = Color.LightBlue;

        private VectorVisualizationComponentDataModel()
        {
        }
        
        public static VectorVisualizationComponentDataModel FromVector3(Vector3 vector, string graphicPassName, Color? color = null)
        {
            return FromVector3(vector, Vector3.Zero, graphicPassName, color);
        }
        
        public static VectorVisualizationComponentDataModel FromVector3(Vector3 vector, Vector3 offset, string graphicPassName, Color? color = null)
        {
            var normal = Vector3.Normalize(vector);
            var vectorLength = vector.Length();
            var scale = new Vector3(vectorLength, 1, 1);
            var rotationAxis = Vector3.Cross(Vector3.UnitX, normal);
            var rotationAngle = (float)Math.Acos(Vector3.Dot(Vector3.UnitX, vector) / vectorLength);

            return new VectorVisualizationComponentDataModel
            {
                Position = offset,
                Scale =  scale,
                Rotation = Matrix.CreateFromAxisAngle(rotationAxis, rotationAngle),
                GraphicsPassName = graphicPassName,
                Color = color ?? DefaultColor
            };
        }
    }
    
    public class VectorVisualizationComponentView<TController> : RenderablePrimitive<VectorVisualizationComponentDataModel, TController> where TController : class, IController
    {
        public VectorVisualizationComponentView(VectorVisualizationComponentDataModel dataModel)
            :base(new FixedSimpleMesh(new ColoredArrowGeometry(dataModel.Color)), dataModel)
        {
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position - Vector3.One * DataModel.Scale.X / 2, DataModel.Position + Vector3.One * DataModel.Scale.X / 2);
        }
    }

    public class VectorVisualizationComponentView : VectorVisualizationComponentView<EmptyController>
    {
        public VectorVisualizationComponentView(VectorVisualizationComponentDataModel dataModel) : base(dataModel)
        {
        }
    }
}