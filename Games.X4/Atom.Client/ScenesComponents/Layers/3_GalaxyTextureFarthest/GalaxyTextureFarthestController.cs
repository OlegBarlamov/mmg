using System;
using System.Threading;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class GalaxyTextureFarthestController : Controller<GalaxyTextureFarthestViewModel3D>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _locker = new object();

        public GalaxyTextureFarthestController(
            [NotNull] GalaxyTextureFarthestViewModel3D viewModel,
            [NotNull] IBackgroundTasksProcessor backgroundTasksProcessor,
            [NotNull] ITextureGeneratorService textureGeneratorService)
        {
            BackgroundTasksProcessor = backgroundTasksProcessor ?? throw new ArgumentNullException(nameof(backgroundTasksProcessor));
            TextureGeneratorService = textureGeneratorService ?? throw new ArgumentNullException(nameof(textureGeneratorService));
            SetModel(viewModel);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            using (_cancellationTokenSource)
            {
                _cancellationTokenSource?.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var aggData = DataModel.AggregatedData;

            BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
            {
                try
                {
                    var texture = GalaxyTextureGenerator.Generate(
                        aggData.ClusterPoints,
                        aggData.DiskRadius,
                        aggData.GalaxyColor,
                        TextureGeneratorService,
                        token);

                    token.ThrowIfCancellationRequested();

                    Texture2D oldTexture;
                    lock (_locker)
                    {
                        oldTexture = aggData.TextureData.Texture;
                        aggData.TextureData.AssignTexture(texture);
                    }
                    oldTexture?.Dispose();
                }
                catch (OperationCanceledException)
                {
                }
            }, _cancellationTokenSource.Token));
        }

        protected override void OnDetached(SceneBase scene)
        {
            base.OnDetached(scene);
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
