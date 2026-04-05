using System;
using System.Threading;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace Atom.Client.Components
{
    public static class GalaxyTextureGenerator
    {
        private static readonly Vector3 ColorBlueWhite = new Vector3(0.7f, 0.8f, 1.0f);
        private static readonly Vector3 ColorWhite = new Vector3(1.0f, 1.0f, 0.95f);
        private static readonly Vector3 ColorYellow = new Vector3(1.0f, 0.9f, 0.6f);
        private static readonly Vector3 ColorOrange = new Vector3(1.0f, 0.7f, 0.4f);
        private static readonly Vector3 ColorRed = new Vector3(1.0f, 0.4f, 0.3f);

        public static Texture2D Generate(
            int armCount,
            int seed,
            Color galaxyColor,
            ITextureGeneratorService textureGeneratorService,
            CancellationToken cancellationToken,
            int textureSize = 64,
            int starCount = 300,
            float armAngleOffset = 0f)
        {
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var rng = new Random(seed);
                var colors = new Color[textureSize, textureSize];
                var center = textureSize / 2f;
                var galaxyTint = galaxyColor.ToVector3();

                cancellationToken.ThrowIfCancellationRequested();

                for (int i = 0; i < starCount; i++)
                {
                    var arm = i % armCount;
                    var armBaseAngle = arm * MathHelper.TwoPi / armCount;

                    var r = (float)(rng.NextDouble() * rng.NextDouble()) * center * 0.9f;
                    var windAngle = r / center * MathHelper.TwoPi * 1.5f;
                    var scatter = (float)(rng.NextDouble() - 0.5) * 2f * (2f + r * 0.15f);

                    var angle = armBaseAngle + windAngle + armAngleOffset;
                    var px = (int)(center + r * (float)Math.Cos(angle) + scatter);
                    var py = (int)(center + r * (float)Math.Sin(angle) + scatter);

                    if (px < 0 || px >= textureSize || py < 0 || py >= textureSize)
                        continue;

                    var distFromCenter = r / (center * 0.9f);
                    var brightness = MathHelper.Clamp(1f - distFromCenter * 0.6f, 0.2f, 1f);

                    var baseColor = RandomStarColor(rng);
                    var blended = Vector3.Lerp(baseColor, galaxyTint, 0.3f) * brightness;
                    var starColor = new Color(blended);

                    var existing = colors[px, py];
                    colors[px, py] = new Color(
                        Math.Min(existing.R + starColor.R, 255),
                        Math.Min(existing.G + starColor.G, 255),
                        Math.Min(existing.B + starColor.B, 255),
                        Math.Min(existing.A + 200, 255));
                }

                cancellationToken.ThrowIfCancellationRequested();

                var bulgeColor = Vector3.Lerp(ColorYellow, galaxyTint, 0.4f);
                for (int x = 0; x < textureSize; x++)
                for (int y = 0; y < textureSize; y++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    var bulge = Math.Max(0f, 1f - dist / (center * 0.25f));
                    if (bulge > 0)
                    {
                        var bc = new Color(bulgeColor * bulge * 0.5f);
                        var c = colors[x, y];
                        colors[x, y] = new Color(
                            Math.Min(c.R + bc.R, 255),
                            Math.Min(c.G + bc.G, 255),
                            Math.Min(c.B + bc.B, 255),
                            Math.Min(c.A + (int)(bulge * 180), 255));
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                texture.SetDataToTexture(colors);
            }
            catch (Exception)
            {
                texture?.Dispose();
                throw;
            }

            return texture;
        }

        private static Vector3 RandomStarColor(Random rng)
        {
            var t = rng.NextDouble();
            if (t < 0.15) return ColorBlueWhite;
            if (t < 0.40) return ColorWhite;
            if (t < 0.70) return ColorYellow;
            if (t < 0.90) return ColorOrange;
            return ColorRed;
        }
    }
}
