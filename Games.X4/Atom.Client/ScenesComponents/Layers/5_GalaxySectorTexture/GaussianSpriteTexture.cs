using System;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace Atom.Client.Components
{
    public static class GaussianSpriteTexture
    {
        private const int Size = 128;
        private const float Falloff = 5.0f;

        private static Texture2D _cached;
        private static readonly object Lock = new object();

        public static Texture2D GetOrCreate(ITextureGeneratorService textureGeneratorService)
        {
            if (_cached != null && !_cached.IsDisposed)
                return _cached;

            lock (Lock)
            {
                if (_cached != null && !_cached.IsDisposed)
                    return _cached;

                _cached = Generate(textureGeneratorService);
                return _cached;
            }
        }

        private static Texture2D Generate(ITextureGeneratorService textureGeneratorService)
        {
            var texture = textureGeneratorService.EmptyTexture(Size, Size);
            var colors = new Color[Size, Size];
            var center = Size / 2f;

            for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
            {
                var dx = (x - center) / center;
                var dy = (y - center) / center;
                var rSq = dx * dx + dy * dy;
                var alpha = (float)Math.Exp(-rSq * Falloff);
                alpha = MathHelper.Clamp(alpha, 0f, 1f);
                var v = (int)(alpha * 255);
                colors[x, y] = new Color(v, v, v, v);
            }

            texture.SetDataToTexture(colors);
            return texture;
        }
    }
}
