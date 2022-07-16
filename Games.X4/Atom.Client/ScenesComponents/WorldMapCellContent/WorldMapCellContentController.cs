using System;
using System.Threading;
using Atom.Client.Services;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class WorldMapCellContentController : Controller<WorldMapCellContent>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private IPlayerProvider PlayerProvider { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }
        
        private CancellationTokenSource _textureGenerationCancellationTokenSource;
        
        private readonly object _locker = new object();

        public WorldMapCellContentController(
            [NotNull] WorldMapCellContent model,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] IPlayerProvider playerProvider,
            [NotNull] ITextureGeneratorService textureGeneratorService)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            PlayerProvider = playerProvider ?? throw new ArgumentNullException(nameof(playerProvider));
            TextureGeneratorService = textureGeneratorService ?? throw new ArgumentNullException(nameof(textureGeneratorService));

            SetModel(model);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            var cameraPosition = PlayerProvider.GetPlayerPosition();
            DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Rotation =
                GetBillboardRotation(DataModel.GetWorldPosition(), cameraPosition, Vector3.Up);
            
            using (_textureGenerationCancellationTokenSource)
            {
                _textureGenerationCancellationTokenSource?.Cancel();
            }

            _textureGenerationCancellationTokenSource = new CancellationTokenSource();
            
            BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
            {
                try
                {
                    var mapPoint = GlobalWorldMap.MapPointFromWorld(cameraPosition);
                    
                    var texture = MapCellTextureGenerator.GenerateAsync(
                        DataModel.GetWorldPosition(),
                        GlobalWorldMap.WorldFromMapPoint(mapPoint),
                        DataModel.WorldMapCellAggregatedData.GalaxiesPoints,
                        TextureGeneratorService, token);

                    token.ThrowIfCancellationRequested();

                    Texture2D oldTexture;
                    lock (_locker)
                    {
                        oldTexture = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Texture;
                        DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData
                            .AssignNewTexture(mapPoint, texture);
                    }
                    oldTexture?.Dispose();
                }
                catch (OperationCanceledException)
                {
                    //ignored
                }
            }, _textureGenerationCancellationTokenSource.Token));
        }

        private Matrix GetBillboardRotation(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp)
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
            return Matrix.CreateRotationX(3 * MathHelper.Pi / 2) * new Matrix
            {
                // Billboard with inverted M11-M12-M13
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

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            _textureGenerationCancellationTokenSource?.Dispose();
        }
    }
}