using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Cameras
{
    public interface ICameraService
    {
        [NotNull] ICamera ActiveCamera { get; }

        void ChangeCamera([NotNull] ICamera camera);
    }
}