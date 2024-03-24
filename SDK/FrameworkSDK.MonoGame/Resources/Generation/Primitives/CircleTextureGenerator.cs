using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using NetExtensions.Helpers;

namespace FrameworkSDK.MonoGame.Resources.Generation.Primitives
{
    internal static class CircleTextureGenerator
    {
        public static Texture2D GenerateCircle(this GraphicsDevice graphicsDevice, int radius, Color fillColor, Color? strokeColor, int? strokeThickness, Color? outerColor)
        {
            var thickness = strokeThickness ?? 0;
            var size = radius * 2 + 1;
            var center = size / 2;
            var texture = graphicsDevice.GetEmptyTexture(size, size);
            var colors = new Color[size, size];
            var sqrFillRadius = MathExtended.Sqr(radius - thickness);
            var sqrRadius = MathExtended.Sqr(radius);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var sqrDistance = MathExtended.Sqr(center - x) + MathExtended.Sqr(center - y);
                    if (sqrDistance <= sqrFillRadius)
                    {
                        colors[x, y] = fillColor;
                    }
                    else if (sqrDistance <= sqrRadius)
                    {
                        colors[x, y] = strokeColor ?? fillColor;
                    } 
                    else
                    {
                        if (outerColor != null)
                            colors[x, y] = outerColor.Value;
                    }
                }
            }
            texture.SetDataToTexture(colors);
            return texture;
        }
    }
}