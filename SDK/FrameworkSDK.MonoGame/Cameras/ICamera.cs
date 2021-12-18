using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Cameras
{
    public interface ICamera : IPosition, IRotation
    {
        Vector3 Up { get; }
        
        Vector3 Target { get; }
        
        Size Viewport { get; }
        
        Matrix GetView();

        Matrix GetProjection();

        Vector2 ToScreen(Vector3 world);

        Vector3 ToWorld(Vector2 screen);
    }
}