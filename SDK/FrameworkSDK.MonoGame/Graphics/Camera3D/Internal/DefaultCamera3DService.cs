using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    [UsedImplicitly]
    internal class DefaultCamera3DService : ICamera3DService, IDisposable
    {
        public const float DefaultAspectRatio = 1.77777777777777f;
        public const float DefaultNearPlaneDistance = 0.1f;
        public const float DefaultFarPlaneDistance = 100f;
        public static readonly float DefaultFieldOfView = (float) Math.PI / 4;
        public static readonly Vector3 DefaultCameraPosition = new Vector3(10, 10, 10);
        public static readonly Vector3 DefaultCameraTargetPoint = Vector3.Zero;

        private static ICamera3D DefaultCamera { get; } = new StaticFixedCamera3D(
            Matrix.CreateLookAt(DefaultCameraPosition, DefaultCameraTargetPoint, Vector3.Up), 
            Matrix.CreatePerspectiveFieldOfView(DefaultFieldOfView, DefaultAspectRatio, DefaultNearPlaneDistance, DefaultFarPlaneDistance));
        
        [NotNull] private ICamera3D _activeCamera = DefaultCamera;

        private readonly ModuleLogger _logger;
        
        public DefaultCamera3DService(IFrameworkLogger logger)
        {
            _logger = new ModuleLogger(logger, LogCategories.Cameras);
        }
        
        public ICamera3D GetActiveCamera()
        {
            return _activeCamera;
        }

        public void SetActiveCamera(ICamera3D camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            if (!ReferenceEquals(camera, _activeCamera))
            {
                var oldCamera = _activeCamera;
                _activeCamera = camera;
                _logger.Info(Strings.Info.Cameras.CameraSwitched, oldCamera, camera);
            }
        }

        public void Dispose()
        {
            _logger.Dispose();
        }
    }
}