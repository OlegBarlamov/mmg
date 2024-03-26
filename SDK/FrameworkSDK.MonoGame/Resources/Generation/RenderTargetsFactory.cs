using System;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Resources.Generation
{
    [UsedImplicitly]
    internal class RenderTargetsFactory : IRenderTargetsFactory
    {
        private IGameHeartServices GameHeartServices { get; }
        private IResourceReferencesService ResourceReferencesService { get; }
        private IDisplayService DisplayService { get; }
        private AppStateService AppStateService { get; }

        private GraphicsDevice GraphicsDevice => GameHeartServices.GraphicsDeviceManager.GraphicsDevice;

        public RenderTargetsFactory(
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IResourceReferencesService resourceReferencesService,
            [NotNull] IDisplayService displayService,
            [NotNull] AppStateService appStateService)
        {
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            ResourceReferencesService = resourceReferencesService ?? throw new ArgumentNullException(nameof(resourceReferencesService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            AppStateService = appStateService ?? throw new ArgumentNullException(nameof(appStateService));
        }
        
        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            return new RenderTarget2D(GraphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared, arraySize);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
        {
            return new RenderTarget2D(GraphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            return new RenderTarget2D(GraphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
        {
            return new RenderTarget2D(GraphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(GraphicsDevice, width, height);
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            return new FullScreenRenderTargetWrapper(
                this,
                DisplayService,
                AppStateService,
                mipMap,
                preferredFormat,
                preferredDepthFormat,
                preferredMultiSampleCount,
                usage,
                shared,
                arraySize);
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
        {
            return CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount,
                usage,  shared,1);
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            return CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount,
                usage,  false,1);
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget(bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat)
        {
            return CreateFullScreenRenderTarget(mipMap, preferredFormat, preferredDepthFormat, 0,
                RenderTargetUsage.DiscardContents,  false,1);
        }

        public IRenderTargetWrapper CreateFullScreenRenderTarget()
        {
            return CreateFullScreenRenderTarget(false, SurfaceFormat.Color, DepthFormat.None, 0,
                RenderTargetUsage.DiscardContents, false, 1);
        }
    }
}