using System;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class PointVisualizationComponentDataModel : ViewModel3D
    {
        public Color Color = Color.DarkRed;
        
        private ColorsTexturesPackage ColorsTexturesPackage { get; }
        
        public PointVisualizationComponentDataModel([NotNull] ColorsTexturesPackage colorsTexturesPackage)
        {
            ColorsTexturesPackage = colorsTexturesPackage ?? throw new ArgumentNullException(nameof(colorsTexturesPackage));
        }

        public Texture2D GetTexture()
        {
            return ColorsTexturesPackage.Get(Color);
        }
    }

    public class PointVisualizationComponentController : Controller<PointVisualizationComponentDataModel>
    {
        private ICamera3DProvider Camera3DProvider { get; }

        private Vector3 _lastCameraPosition = new Vector3(float.MinValue,float.MinValue,float.MinValue);

        public PointVisualizationComponentController([NotNull] ICamera3DProvider camera3DProvider)
        {
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var cameraPosition = Camera3DProvider.GetActiveCamera().GetPosition();
            if (cameraPosition != _lastCameraPosition)
            {
                _lastCameraPosition = cameraPosition;

                var pointPosition = DataModel.Position;
                var cameraDistance = Vector3.Distance(pointPosition, cameraPosition);

                DataModel.Scale = new Vector3(0.0075f) * cameraDistance;
            }
        }
    }
    
    public class PointVisualizationComponentView : RenderablePrimitive<PointVisualizationComponentDataModel, PointVisualizationComponentController>
    {
        public PointVisualizationComponentView(PointVisualizationComponentDataModel dataModel)
            :base(new FixedSimpleMesh(new TetrahedronGeometry()), dataModel)
        {
            Mesh.Material = new TextureMaterial(DataModel.GetTexture());
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return new BoundingBox(DataModel.Position, DataModel.Position);
        }
    }
}