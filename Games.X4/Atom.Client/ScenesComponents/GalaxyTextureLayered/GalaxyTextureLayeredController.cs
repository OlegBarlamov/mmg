using System;
using System.Threading;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    [UsedImplicitly]
    public class GalaxyTextureLayeredController : Controller<GalaxyTextureLayeredViewModel3D>
    {
        [NotNull] private IBackgroundTasksProcessor BackgroundTasksProcessor { get; }
        [NotNull] private ITextureGeneratorService TextureGeneratorService { get; }

        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _locker = new object();

        public GalaxyTextureLayeredController(
            [NotNull] GalaxyTextureLayeredViewModel3D viewModel,
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

            var layerCount = GalaxyTextureLayeredAggregatedData.LayerCount;
            var angleStep = layerCount > 1 ? MathHelper.TwoPi / layerCount : 0f;

            for (int i = 0; i < layerCount; i++)
            {
                var layerIndex = i;
                var angleOffset = layerCount > 1 ? (i - (layerCount - 1) / 2f) * angleStep : 0f;

                BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
                {
                    try
                    {
                        var texture = GalaxyTextureGenerator.Generate(
                            aggData.Sectors,
                            aggData.DiskRadius,
                            aggData.GalaxyColor,
                            TextureGeneratorService,
                            token,
                            textureSize: 512,
                            armAngleOffset: angleOffset);

                        token.ThrowIfCancellationRequested();

                        Texture2D oldTexture;
                        lock (_locker)
                        {
                            oldTexture = aggData.LayerTextures[layerIndex].Texture;
                            aggData.LayerTextures[layerIndex].AssignTexture(texture);
                        }
                        oldTexture?.Dispose();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }, _cancellationTokenSource.Token));
            }
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
