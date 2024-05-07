using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Resources;
using FrameworkSDK.MonoGame.Resources.Generation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Geometry;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.Services
{
    [UsedImplicitly]
    internal class RenderTargetsFactoryService : IRenderTargetsFactoryService
    {
        private IRenderTargetsFactory RenderTargetsFactory { get; }
        private IResourceReferencesService ResourceReferencesService { get; }

        public RenderTargetsFactoryService([NotNull] IRenderTargetsFactory renderTargetsFactory,
            [NotNull] IResourceReferencesService resourceReferencesService)
        {
            RenderTargetsFactory = renderTargetsFactory ?? throw new ArgumentNullException(nameof(renderTargetsFactory));
            ResourceReferencesService = resourceReferencesService ?? throw new ArgumentNullException(nameof(resourceReferencesService));
        }

        public RenderTarget2D CreateRenderTarget(int width, int height, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            var renderTarget = RenderTargetsFactory.CreateRenderTarget(width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared, arraySize);
            SaveReference(renderTarget);
            return renderTarget;
        }

        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            var renderTarget = RenderTargetsFactory.CreateRenderTarget(width, height);
            SaveReference(renderTarget);
            return renderTarget;
        }

        public IRenderTargetWrapper CreateDisplaySizedRenderTarget(Func<SizeInt, SizeInt> getSize, bool mipMap, SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared,
            int arraySize)
        {
            var renderTarget = RenderTargetsFactory.CreateDisplaySizedRenderTarget(getSize, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared, arraySize);
            SaveReference(renderTarget);
            return renderTarget;
        }

        private void SaveReference(RenderTarget2D renderTarget2D)
        {
            ResourceReferencesService.AddPackageless(renderTarget2D);
            renderTarget2D.Disposing += RenderTarget2DOnDisposing;
        }

        private void SaveReference(IDisposableExtended disposableExtended)
        {
            ResourceReferencesService.AddPackageless(disposableExtended);
            disposableExtended.DisposedEvent += DisposableExtendedOnDisposed;
        }

        private void RenderTarget2DOnDisposing(object sender, EventArgs e)
        {
            var renderTarget = (RenderTarget2D) sender;
            renderTarget.Disposing -= RenderTarget2DOnDisposing;
            ResourceReferencesService.RemovePackageless(renderTarget);
        }
        
        private void DisposableExtendedOnDisposed(object sender, EventArgs e)
        {
            var disposable = (IDisposableExtended) sender;
            disposable.DisposedEvent -= DisposableExtendedOnDisposed;
            ResourceReferencesService.RemovePackageless(disposable);
        }
    }
}