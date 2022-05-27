using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public interface ICamera3DService : ICamera3DProvider
    {
        void SetActiveCamera([NotNull] ICamera3D camera);
    }
}