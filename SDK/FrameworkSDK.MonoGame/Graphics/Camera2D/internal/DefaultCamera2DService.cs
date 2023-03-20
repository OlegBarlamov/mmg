using System;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using NetExtensions.Geometry;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public class DefaultCamera2DService : ICamera2DService, IDisposable
    {
        private IDisplayService DisplayService { get; }
        
        private ICamera2D _activeCamera;
        private readonly ScreenEquivalentCamera2D _screenEquivalentCamera;
        
        public DefaultCamera2DService([NotNull] IDisplayService displayService)
        {
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            _screenEquivalentCamera = new ScreenEquivalentCamera2D(DisplayService);
        }
        
        public void Dispose()
        {
            _screenEquivalentCamera.Dispose();
        }

        public ICamera2D GetScreenCamera()
        {
            return _screenEquivalentCamera;
        }

        public ICamera2D GetActiveCamera()
        {
            return _activeCamera ?? _screenEquivalentCamera;
        }

        public void SetActiveCamera(ICamera2D camera)
        {
            _activeCamera = camera;
        }
    }
}