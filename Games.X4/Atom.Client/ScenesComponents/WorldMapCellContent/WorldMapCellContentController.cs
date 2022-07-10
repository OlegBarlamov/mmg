using System;
using System.Threading;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class WorldMapCellContentController : Controller<WorldMapCellContent>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private ICamera3DProvider Camera3DProvider { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }
        private IRandomService RandomService { get; }

        private CancellationTokenSource _textureGenerationCancellationTokenSource;

        public WorldMapCellContentController(
            [NotNull] WorldMapCellContent model,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] ICamera3DProvider camera3DProvider,
            [NotNull] ITextureGeneratorService textureGeneratorService,
            [NotNull] IRandomService randomService)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            Camera3DProvider = camera3DProvider ?? throw new ArgumentNullException(nameof(camera3DProvider));
            TextureGeneratorService = textureGeneratorService ?? throw new ArgumentNullException(nameof(textureGeneratorService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            SetModel(model);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            var cameraPosition = Camera3DProvider.GetActiveCamera().GetPosition();
            var mapPoint = GlobalWorldMap.MapPointFromWorld(cameraPosition);
            if (!DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.IsTextureExist ||
                mapPoint != DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.TextureTargetPoint)
            {
                using (_textureGenerationCancellationTokenSource)
                {
                    _textureGenerationCancellationTokenSource?.Cancel(); 
                }
                _textureGenerationCancellationTokenSource = new CancellationTokenSource();

                BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
                {
                    try
                    {
                        var texture = MapCellTextureGenerator.GenerateAsync(
                            DataModel.GetWorldPosition(),
                            GlobalWorldMap.WorldFromMapPoint(mapPoint),
                            DataModel.WorldMapCellAggregatedData.GalaxiesPoints,
                            TextureGeneratorService, RandomService, token);
                        
                        token.ThrowIfCancellationRequested();

                        var oldTexture = DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.Texture; 
                        DataModel.WorldMapCellAggregatedData.WorldMapCellTextureData.AssignNewTexture(mapPoint, texture);
                        oldTexture?.Dispose();
                    }
                    catch (OperationCanceledException)
                    {
                        //ignored
                    }
                }, _textureGenerationCancellationTokenSource.Token));
            }
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            _textureGenerationCancellationTokenSource?.Dispose();
        }
    }
}