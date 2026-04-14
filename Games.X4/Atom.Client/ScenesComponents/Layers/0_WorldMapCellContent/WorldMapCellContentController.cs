using System;
using System.Threading;
using Atom.Client.ScenesComponents.WorldMapCellContent;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Maps;

namespace Atom.Client.Components
{
    public class WorldMapCellContentController : BillboardController<WorldMapCellContentViewModel3D>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }
        
        private CancellationTokenSource _textureGenerationCancellationTokenSource;
        
        private readonly object _locker = new object();

        protected override bool ContinuouslyUpdateRotation => false;

        public WorldMapCellContentController(
            [NotNull] WorldMapCellContentViewModel3D model,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] ICamera3DProvider camera3DProvider,
            [NotNull] ITextureGeneratorService textureGeneratorService)
            : base(model, camera3DProvider)
        {
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            TextureGeneratorService = textureGeneratorService ?? throw new ArgumentNullException(nameof(textureGeneratorService));
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            var cameraPosition = Camera3DProvider.GetActiveCamera().GetPosition();

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
                        DataModel.WorldPosition,
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

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            
            _textureGenerationCancellationTokenSource?.Dispose();
        }
    }
}
