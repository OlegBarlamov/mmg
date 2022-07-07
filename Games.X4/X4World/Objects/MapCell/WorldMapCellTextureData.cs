using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Geometry;

namespace X4World.Objects
{
    public class WorldMapCellTextureData : IDisposable
    {
        public event Action TextureChanged;
        
        public bool IsTextureExist => Texture != null;
        [CanBeNull] public Texture2D Texture { get; private set; }
        
        public Point3D TextureTargetPoint { get; private set; } = new Point3D(int.MinValue, int.MinValue, int.MinValue);

        public void AssignNewTexture(Point3D targetPoint, [NotNull] Texture2D texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            TextureTargetPoint = targetPoint;
            
            TextureChanged?.Invoke();
        }
        
        public void Dispose()
        {
            TextureChanged = null;
        }
    }
}