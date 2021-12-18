using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Cameras
{
    public class StaticCamera : ICamera
    {
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        public Vector3 Up { get; }
        public Vector3 Target { get; }
        public Size Viewport { get; }

        public Matrix GetView()
        {
            throw new System.NotImplementedException();
        }

        public Matrix GetProjection()
        {
            throw new System.NotImplementedException();
        }

        public Vector2 ToScreen(Vector3 world)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 ToWorld(Vector2 screen)
        {
            throw new System.NotImplementedException();
        }
    }
}