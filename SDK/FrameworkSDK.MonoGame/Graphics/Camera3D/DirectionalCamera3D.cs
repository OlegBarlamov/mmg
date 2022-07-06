using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public class DirectionalCamera3D : PerspectiveCamera3D
    {
        public override string Name { get; set; } = nameof(DirectionalCamera3D);

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateMatrices();
            }
        }


        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                UpdateMatrices();
            }
        }
        
        public Vector3 Up
        {
            get => _up;
            set
            {
                _up = value;
                UpdateMatrices();
            }
        }
        
        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _up = Vector3.Up;
        private readonly BoundingFrustum _boundingFrustum;

        public DirectionalCamera3D(Vector3 position, Vector3 target)
        {
            _position = position;
            _target = target;
            _boundingFrustum = new BoundingFrustum(Matrix.Identity);
            
            UpdateMatrices();
        }
        
        public override bool CheckBoundingBoxVisible(BoundingBox boundingBox)
        {
            var containmentType = _boundingFrustum.Contains(boundingBox);
            return containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects;
        }

        public override Vector3 GetPosition()
        {
            return _position;
        }

        protected sealed override void UpdateMatrices()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlaneDistance, FarPlaneDistance);
            View = Matrix.CreateLookAt(Position, Target, Up);
            _boundingFrustum.Matrix = View * Projection;
        }
    }
}