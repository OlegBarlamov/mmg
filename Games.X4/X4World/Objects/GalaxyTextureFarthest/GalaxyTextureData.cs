using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace X4World.Objects
{
    public class GalaxyTextureData : IDisposable
    {
        public event Action TextureChanged;

        [CanBeNull] public Texture2D Texture { get; private set; }

        public void AssignTexture([NotNull] Texture2D texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            TextureChanged?.Invoke();
        }

        public void Dispose()
        {
            TextureChanged = null;
            Texture?.Dispose();
            Texture = null;
        }
    }
}
