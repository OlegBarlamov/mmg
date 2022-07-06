using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public interface ICamera3D : INamed
    {
        Vector3 GetPosition();
        Matrix GetProjection();
        Matrix GetView();
        bool CheckBoundingBoxVisible(BoundingBox boundingBox);
    }
}
