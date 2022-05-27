
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public class StaticFixedCamera3D : ICamera3D
    {
        public string Name { get; set; } = nameof(StaticFixedCamera3D);
        
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public StaticFixedCamera3D(Matrix view, Matrix projection)
        {
            View = view;
            Projection = projection;
        }
        
        public Matrix GetProjection()
        {
            return Projection;
        }

        public Matrix GetView()
        {
            return View;
        }
    }
}