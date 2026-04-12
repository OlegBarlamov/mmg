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
    public class GalaxySectorSlabController : Controller<GalaxySectorSlabViewModel3D>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }

        private CancellationTokenSource _cancellationTokenSource;

        public GalaxySectorSlabController(
            [NotNull] GalaxySectorSlabViewModel3D viewModel,
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

            var parentAgg = DataModel.ParentAggregatedData;
            var slabTextureData = parentAgg.SlabTextureData;

            if (slabTextureData.Texture != null)
            {
                DataModel.OnSlabTextureAvailable(slabTextureData.Texture);
                return;
            }

            using (_cancellationTokenSource)
            {
                _cancellationTokenSource?.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
            {
                try
                {
                    if (slabTextureData.Texture != null)
                        return;

                    var texture = GalaxyTextureGenerator.Generate(
                        parentAgg.Sectors,
                        parentAgg.DiskRadius,
                        parentAgg.GalaxyColor,
                        TextureGeneratorService,
                        token,
                        textureSize: 256,
                        extraBlurRadius: 4);

                    token.ThrowIfCancellationRequested();

                    if (slabTextureData.Texture == null)
                        slabTextureData.AssignTexture(texture);
                    else
                        texture.Dispose();
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
