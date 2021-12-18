using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Cameras
{
    [UsedImplicitly]
    internal class CameraService : ICameraService
    {
        public ICamera ActiveCamera { get; private set; }
        
        private static ModuleLogger Logger { get; } = new ModuleLogger(AppContext.Logger, FrameworkLogModule.Cameras);
        
        public void ChangeCamera(ICamera camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            
            var oldCamera = ActiveCamera;
            if (!ReferenceEquals(oldCamera, camera))
            {
                ActiveCamera = camera;
                Logger.Info(Strings.Info.Cameras.CameraSwitched,oldCamera, camera);
            }
        }
    }
}