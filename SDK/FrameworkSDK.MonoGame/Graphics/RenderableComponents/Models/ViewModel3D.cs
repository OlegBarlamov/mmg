using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Materials;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models
{
    public abstract class ViewModel3D : ViewModel, IPlaceable3DReactive, IDisposable
    {
        public event EventHandler PlacementChanged;
            
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                PlacementChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                PlacementChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Matrix Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                PlacementChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IMeshMaterial MeshMaterial { get; set; } = StaticMaterials.EmptyMaterial;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _scale = Vector3.One;
        private Matrix _rotation = Matrix.Identity;
        
        public void Dispose()
        {
            PlacementChanged = null;
        }
    }
}