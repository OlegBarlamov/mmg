using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public interface ICamera2DProvider
    {
        [NotNull] ICamera2D GetScreenCamera();
        [NotNull] ICamera2D GetActiveCamera();
    }
}