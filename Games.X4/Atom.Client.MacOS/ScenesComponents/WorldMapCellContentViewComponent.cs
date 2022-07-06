using System;
using System.Threading;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.Materials;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.SceneComponents.Geometries;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Components
{
    public class WorldMapCellContentController : Controller<WorldMapCellContent>
    {
        public event Action TextureIsReady;
        public IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        public ICamera3DProvider Camera3DProvider { get; }
        [NotNull] public ITextureGeneratorService TextureGeneratorService { get; }
        private bool _isGenerationStarted;

        public WorldMapCellContentController(WorldMapCellContent model, [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] ICamera3DProvider camera3DProvider, [NotNull] ITextureGeneratorService textureGeneratorService)
        {
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
            TextureGeneratorService = textureGeneratorService ?? throw new ArgumentNullException(nameof(textureGeneratorService));
            SetModel(model);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!DataModel.WorldMapCellAggregatedData.IsTextureGenerated && !_isGenerationStarted)
            {
                _isGenerationStarted = true;
                BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time =>
                {
                    try
                    {
                        DataModel.WorldMapCellAggregatedData.GenerateTexture(DataModel.GetWorldPosition(), TextureGeneratorService);
                        TextureIsReady?.Invoke();
                    }
                    finally
                    {
                        _isGenerationStarted = false;
                    }
                }), CancellationToken.None));
            }
        }
    }
    
    public sealed class WorldMapCellContentViewComponent : SingleMeshComponent<WorldMapCellContent, WorldMapCellContentController>
    {
        public WorldMapCellContentViewComponent(WorldMapCellContent dataModel, ICamera3DProvider camera3DProvider)
            : base(new FixedSimpleMesh(StaticGeometries.Plane), "Render_Textured_No_Lights")
        {
            SetDataModel(dataModel);

            var cameraPosition = camera3DProvider.GetActiveCamera().GetPosition();
            var cameraMapCellCenter = GlobalWorldMap.WorldFromMapPoint(GlobalWorldMap.MapPointFromWorld(cameraPosition));
            var objectPosition = DataModel.GetWorldPosition();
            Mesh.Position = objectPosition;
            Mesh.Scale = DataModel.Size;
            Mesh.Rotation = GetRotation(objectPosition, cameraMapCellCenter, Vector3.Up);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            Controller.TextureIsReady += ControllerOnTextureIsReady;
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            Controller.TextureIsReady -= ControllerOnTextureIsReady;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Controller.TextureIsReady -= ControllerOnTextureIsReady;
        }

        private Matrix GetRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
        {
            var normal = cameraPosition - objectPosition;
            normal.Normalize();
            if (normal == Vector3.Up)
                return Matrix.Identity;
            if (normal == Vector3.Down)
                return Matrix.CreateRotationX(MathHelper.Pi);
            
            var rotationAxis = Vector3.Cross(cameraUp, normal);
            rotationAxis.Normalize();
            var down = Vector3.Cross(normal, rotationAxis);
            return Matrix.CreateRotationX(-MathHelper.Pi / 2) * new Matrix
            {
                M11 = -rotationAxis.X,
                M12 = -rotationAxis.Y,
                M13 = -rotationAxis.Z,
                M14 = 0.0f,
                M21 = down.X,
                M22 = down.Y,
                M23 = down.Z,
                M24 = 0.0f,
                M31 = normal.X,
                M32 = normal.Y,
                M33 = normal.Z,
                M34 = 0.0f,
                M41 = 0f,
                M42 = 0f,
                M43 = 0f,
                M44 = 1f
            };
        }

        private void ControllerOnTextureIsReady()
        {
            Controller.TextureIsReady -= ControllerOnTextureIsReady;
            Mesh.Material = new TextureMaterial(DataModel.WorldMapCellAggregatedData.Texture);
        }

        protected override BoundingBox? ConstructBoundingBox()
        {
            return
                null; //new BoundingBox(DataModel.GetWorldPosition() - DataModel.Size / 2, DataModel.GetWorldPosition() + DataModel.Size / 2);
        }
    }
}