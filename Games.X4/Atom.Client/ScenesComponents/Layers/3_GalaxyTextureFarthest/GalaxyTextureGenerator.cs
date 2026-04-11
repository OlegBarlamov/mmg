using System;
using System.Threading;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using X4World.Objects;

namespace Atom.Client.Components
{
    public static class GalaxyTextureGenerator
    {
        private static readonly Vector3 ColorYellow = new Vector3(1.0f, 0.9f, 0.6f);

        public static Texture2D Generate(
            GalaxyClusterPoint[] brightStars,
            float diskRadius,
            Color galaxyColor,
            ITextureGeneratorService textureGeneratorService,
            CancellationToken cancellationToken,
            int textureSize = 64)
        {
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var colors = new Color[textureSize, textureSize];
                RenderStars(brightStars, 0f, 0f, diskRadius, galaxyColor, colors, textureSize);
                cancellationToken.ThrowIfCancellationRequested();
                RenderBulge(colors, textureSize, galaxyColor);
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

        public static Texture2D Generate(
            GalaxySectorDefinition[] sectors,
            float diskRadius,
            Color galaxyColor,
            ITextureGeneratorService textureGeneratorService,
            CancellationToken cancellationToken,
            int textureSize = 64)
        {
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var colors = new Color[textureSize, textureSize];
                foreach (var sector in sectors)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    RenderStars(sector.ClusterPoints, sector.CenterX, sector.CenterZ,
                        diskRadius, galaxyColor, colors, textureSize);
                }
                cancellationToken.ThrowIfCancellationRequested();
                RenderBulge(colors, textureSize, galaxyColor);
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

        private static void RenderStars(
            GalaxyClusterPoint[] stars, float offsetX, float offsetZ,
            float diskRadius, Color galaxyColor,
            Color[,] colors, int textureSize)
        {
            var center = textureSize / 2f;
            var scale = center / diskRadius;
            var galaxyTint = galaxyColor.ToVector3();

            for (int i = 0; i < stars.Length; i++)
            {
                var star = stars[i];
                var sx = star.X + offsetX;
                var sz = star.Z + offsetZ;

                var px = (int)(center + sz * scale);
                var py = (int)(center + sx * scale);

                if (px < 0 || px >= textureSize || py < 0 || py >= textureSize)
                    continue;

                var distFromCenter = (float)Math.Sqrt(sx * sx + sz * sz) / (diskRadius * 0.9f);
                var brightness = MathHelper.Clamp(1f - distFromCenter * 0.6f, 0.2f, 1f);

                var starColorVec = GalaxyAsPointAggregatedData.ColorFromTemperature(star.Temperature).ToVector3();
                var blended = Vector3.Lerp(starColorVec, galaxyTint, 0.3f) * brightness;
                var starColor = new Color(blended);

                var existing = colors[px, py];
                colors[px, py] = new Color(
                    Math.Min(existing.R + starColor.R, 255),
                    Math.Min(existing.G + starColor.G, 255),
                    Math.Min(existing.B + starColor.B, 255),
                    Math.Min(existing.A + 200, 255));
            }
        }

        private static void RenderBulge(Color[,] colors, int textureSize, Color galaxyColor)
        {
            var center = textureSize / 2f;
            var galaxyTint = galaxyColor.ToVector3();
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
        }
    }
}
