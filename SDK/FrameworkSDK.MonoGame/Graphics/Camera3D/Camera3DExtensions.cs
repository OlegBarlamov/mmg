using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public static class Camera3DExtensions
    {
        public static Ray CreatePickRay(this ICamera3D camera, Point screenPosition, Viewport viewport)
        {
            var view = camera.GetView();
            var projection = camera.GetProjection();
            var nearPoint = viewport.Unproject(
                new Vector3(screenPosition.X, screenPosition.Y, 0f),
                projection, view, Matrix.Identity);
            var midPoint = viewport.Unproject(
                new Vector3(screenPosition.X, screenPosition.Y, 0.1f),
                projection, view, Matrix.Identity);
            return new Ray(nearPoint, Vector3.Normalize(midPoint - nearPoint));
        }
    }
}