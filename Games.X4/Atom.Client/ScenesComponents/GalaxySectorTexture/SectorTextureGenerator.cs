using System;
using System.Threading;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using X4World.Objects;

namespace Atom.Client.Components
{
    public static class SectorTextureGenerator
    {
        private const int StarRadius = 2;

        public static Texture2D Generate(
            GalaxySectorTextureAggregatedData aggData,
            ITextureGeneratorService textureGeneratorService,
            CancellationToken cancellationToken,
            int textureSize = 256)
        {
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var colors = new Color[textureSize, textureSize];
                var center = textureSize / 2f;
                var scale = center / aggData.SectorRadius;
                var galaxyTint = aggData.GalaxyColor.ToVector3();
                var diskRadius09 = aggData.DiskRadius * 0.9f;

                cancellationToken.ThrowIfCancellationRequested();

                RenderClusterPoints(aggData.ClusterPoints, aggData, colors, textureSize, center, scale, galaxyTint, diskRadius09);

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

        private static void RenderClusterPoints(
            GalaxyClusterPoint[] clusterPoints, GalaxySectorTextureAggregatedData aggData,
            Color[,] colors, int textureSize, float center, float scale,
            Vector3 galaxyTint, float diskRadius09)
        {
            for (int i = 0; i < clusterPoints.Length; i++)
            {
                var p = clusterPoints[i];

                var px = (int)(center + p.Z * scale);
                var py = (int)(center + p.X * scale);

                if (px < 0 || px >= textureSize || py < 0 || py >= textureSize)
                    continue;

                var galaxyX = p.X + aggData.SectorCenterX;
                var galaxyZ = p.Z + aggData.SectorCenterZ;
                var distFromCenter = (float)Math.Sqrt(galaxyX * galaxyX + galaxyZ * galaxyZ) / diskRadius09;
                var brightness = MathHelper.Clamp(1f - distFromCenter * 0.6f, 0.2f, 1f);

                var starColor = GalaxyAsPointAggregatedData.ColorFromTemperature(p.Temperature);
                var blended = Vector3.Lerp(starColor.ToVector3(), galaxyTint, 0.3f) * brightness;

                AddStar(colors, textureSize, px, py, new Color(blended), brightness, StarRadius);
            }
        }

        private static void AddStar(Color[,] colors, int textureSize, int cx, int cy, Color c, float brightness, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            for (int dy = -radius; dy <= radius; dy++)
            {
                var px = cx + dx;
                var py = cy + dy;
                if (px < 0 || px >= textureSize || py < 0 || py >= textureSize)
                    continue;

                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist > radius)
                    continue;

                var falloff = 1f - dist / (radius + 1f);
                var pixelBrightness = brightness * falloff;

                var existing = colors[px, py];
                colors[px, py] = new Color(
                    Math.Min(existing.R + (int)(c.R * falloff), 255),
                    Math.Min(existing.G + (int)(c.G * falloff), 255),
                    Math.Min(existing.B + (int)(c.B * falloff), 255),
                    Math.Min(existing.A + (int)(pixelBrightness * 255), 255));
            }
        }
    }
}
