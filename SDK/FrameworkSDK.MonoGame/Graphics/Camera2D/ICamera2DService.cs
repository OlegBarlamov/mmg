using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Camera2D
{
    public interface ICamera2DService : ICamera2DProvider
    {
        void SetActiveCamera([NotNull] ICamera2D camera);
    }
}