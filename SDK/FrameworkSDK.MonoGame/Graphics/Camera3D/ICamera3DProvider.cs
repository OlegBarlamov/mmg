using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public interface ICamera3DProvider
    {
        [NotNull] ICamera3D GetActiveCamera();
    }
}