using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public interface ICamera3D : INamed
    {
        Matrix GetProjection();
        Matrix GetView();
    }
}
