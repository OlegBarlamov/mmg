using System;
using FrameworkSDK.MonoGame.Resources.Generation;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Geometry;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics
{
    internal class DisplaySizedRenderTargetWrapper : IRenderTargetWrapper
    {
        public event EventHandler DisposedEvent;
        public bool IsDisposed { get; private set; }

        public RenderTarget2D RenderTarget { get; private set; }
        
        private IRenderTargetsFactory RenderTargetsFactory { get; }
        private IDisplayService DisplayService { get; }
        private AppStateService AppStateService { get; }
        private Func<SizeInt, SizeInt> GetSizeFromDisplaySize { get; }

        private RenderTarget2D _newRenderTarget;
        
        private readonly object _deviceResetLocker = new object();
        private readonly RenderTargetParameters _renderTargetParameters;

        public DisplaySizedRenderTargetWrapper(
            [NotNull] IRenderTargetsFactory renderTargetsFactory,
            [NotNull] IDisplayService displayService,
            [NotNull] AppStateService appStateService,
            [NotNull] Func<SizeInt, SizeInt> getSizeFromDisplaySize,
            bool mipMap,
            SurfaceFormat preferredFormat,
            DepthFormat preferredDepthFormat,
            int preferredMultiSampleCount,
            RenderTargetUsage usage,
            bool shared,
            int arraySize)
        {
            RenderTargetsFactory = renderTargetsFactory ?? throw new ArgumentNullException(nameof(renderTargetsFactory));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            AppStateService = appStateService ?? throw new ArgumentNullException(nameof(appStateService));
            GetSizeFromDisplaySize = getSizeFromDisplaySize ?? throw new ArgumentNullException(nameof(getSizeFromDisplaySize));

            _renderTargetParameters = new RenderTargetParameters
            {
                MipMap = mipMap,
                PreferredFormat = preferredFormat,
                PreferredDepthFormat = preferredDepthFormat,
                PreferredMultiSampleCount = preferredMultiSampleCount,
                Usage = usage,
                Shared = shared,
                ArraySize = arraySize
            };

            RenderTarget = CreateRenderTarget();
            DisplayService.DeviceReset += DisplayServiceOnDeviceReset;
        }

        public void Dispose()
        {
            IsDisposed = true;
            DisplayService.DeviceReset -= DisplayServiceOnDeviceReset;
            DisposedEvent?.Invoke(this, EventArgs.Empty);
            DisposedEvent = null;
        }
        
        private void DisplayServiceOnDeviceReset()
        {
            lock (_deviceResetLocker)
            {
                _newRenderTarget?.Dispose();
                _newRenderTarget = CreateRenderTarget();
                
                if (AppStateService.IsDrawStateActive)
                {
                    AppStateService.QueueOnUpdate(gameTime => SwitchRenderTarget(_newRenderTarget));
                }
                else
                {
                    SwitchRenderTarget(_newRenderTarget);
                }
            }
        }

        private SizeInt GetSize()
        {
            return GetSizeFromDisplaySize.Invoke(new SizeInt(DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight));
        }

        private RenderTarget2D CreateRenderTarget()
        {
            var size = GetSize();
            return RenderTargetsFactory.CreateRenderTarget(
                size.Width,
                size.Height,
                _renderTargetParameters.MipMap,
                _renderTargetParameters.PreferredFormat,
                _renderTargetParameters.PreferredDepthFormat,
                _renderTargetParameters.PreferredMultiSampleCount,
                _renderTargetParameters.Usage,
                _renderTargetParameters.Shared,
                _renderTargetParameters.ArraySize);
        }

        private void SwitchRenderTarget(RenderTarget2D newRenderTarget)
        {
            // check for concurrency
            if (ReferenceEquals(_newRenderTarget, newRenderTarget))
            {
                var oldRenderTarget = RenderTarget;
                RenderTarget = newRenderTarget;
                oldRenderTarget.Dispose();
            }
        }

        private class RenderTargetParameters
        {
            public bool MipMap { get; set; }
            public SurfaceFormat PreferredFormat { get; set; }
            public DepthFormat PreferredDepthFormat { get; set; }
            public int PreferredMultiSampleCount { get; set; }
            public RenderTargetUsage Usage { get; set; }
            public bool Shared { get; set; }
            public int ArraySize { get; set; }
        }
    }
}