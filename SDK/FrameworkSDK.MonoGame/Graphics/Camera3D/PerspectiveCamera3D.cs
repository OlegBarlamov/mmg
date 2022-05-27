using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Camera3D
{
    public abstract class PerspectiveCamera3D : ICamera3D
    {
        public abstract string Name { get; set; }

        public float FieldOfView
        {
            get => _fieldOfView;
            set
            {
                _fieldOfView = value;
                UpdateMatrices();
            }
        }

        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                UpdateMatrices();
            }
        }

        public float NearPlaneDistance
        {
            get => _nearPlaneDistance;
            set
            {
                _nearPlaneDistance = value;
                UpdateMatrices();
            }
        }

        public float FarPlaneDistance
        {
            get => _farPlaneDistance;
            set
            {
                _farPlaneDistance = value;
                UpdateMatrices();
            }
        }

        protected Matrix Projection;
        protected Matrix View;

        private float _fieldOfView = DefaultCamera3DService.DefaultFieldOfView;
        private float _aspectRatio = DefaultCamera3DService.DefaultAspectRatio;
        private float _nearPlaneDistance = DefaultCamera3DService.DefaultNearPlaneDistance;
        private float _farPlaneDistance = DefaultCamera3DService.DefaultFarPlaneDistance;
        
        public Matrix GetProjection()
        {
            return Projection;
        }

        public Matrix GetView()
        {
            return View;
        }
        
        protected abstract void UpdateMatrices();
    }
}