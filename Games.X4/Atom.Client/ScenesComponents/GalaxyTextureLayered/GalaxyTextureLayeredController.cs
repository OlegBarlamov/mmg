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

            var angleStep = MathHelper.TwoPi / GalaxyTextureLayeredAggregatedData.LayerCount;

            for (int i = 0; i < GalaxyTextureLayeredAggregatedData.LayerCount; i++)
            {
                var layerIndex = i;
                var layerSeed = aggData.LayerSeeds[i];
                var starCount = aggData.LayerStarCounts[i];
                var angleOffset = (i - 1) * angleStep;

                BackgroundTasksProcessor.EnqueueTask(new SimpleDelayedTask((time, token) =>
                {
                    try
                    {
                        var texture = GalaxyTextureGenerator.Generate(
                            aggData.ArmCount,
                            layerSeed,
                            aggData.GalaxyColor,
                            TextureGeneratorService,
                            token,
                            textureSize: 512,
                            starCount: starCount,
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
