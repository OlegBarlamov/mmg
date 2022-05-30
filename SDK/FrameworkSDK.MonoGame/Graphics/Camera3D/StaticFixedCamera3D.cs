
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public class StaticFixedCamera3D : ICamera3D
    {
        public string Name { get; set; } = nameof(StaticFixedCamera3D);
        
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        private readonly BoundingFrustum _boundingFrustum;
        
        public StaticFixedCamera3D(Matrix view, Matrix projection)
        {
            View = view;
            Projection = projection;
            _boundingFrustum = new BoundingFrustum(View * Projection);
        }
        
        public Matrix GetProjection()
        {
            return Projection;
        }

        public Matrix GetView()
        {
            return View;
        }

        public bool CheckBoundingBoxVisible(BoundingBox boundingBox)
        {
            _boundingFrustum.Matrix = View * Projection;
            var containmentType = _boundingFrustum.Contains(boundingBox);
            return containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects;
        }
    }
}