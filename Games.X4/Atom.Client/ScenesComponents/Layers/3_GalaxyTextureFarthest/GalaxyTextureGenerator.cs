using System;
using System.Threading;
using FrameworkSDK.MonoGame.Resources.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using X4World.Generation;
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
            var cfg = GalaxyConfig.Instance.GalaxyTextureFarthest.Node;
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var colors = new Color[textureSize, textureSize];

                RenderDiffuseDisk(colors, textureSize, galaxyColor,
                    cfg.DiffuseDiskBrightness, cfg.DiffuseDiskAlpha, cfg.DiffuseDiskExtent);
                cancellationToken.ThrowIfCancellationRequested();

                RenderStars(brightStars, 0f, 0f, diskRadius, galaxyColor, colors, textureSize,
                    cfg.TextureBrightnessScale, cfg.TextureStarAlpha, cfg.TextureStarRadius);
                cancellationToken.ThrowIfCancellationRequested();

                ApplyBlur(colors, textureSize, cfg.TextureBlurRadius);
                cancellationToken.ThrowIfCancellationRequested();

                RenderBulge(colors, textureSize, galaxyColor,
                    cfg.BulgeExtent, cfg.BulgeIntensity, cfg.BulgeAlphaScale);
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
            int textureSize = 64,
            int extraBlurRadius = 0)
        {
            var cfg = GalaxyConfig.Instance.GalaxyTextureLayered.Node;
            var texture = textureGeneratorService.EmptyTexture(textureSize, textureSize);
            try
            {
                var colors = new Color[textureSize, textureSize];

                RenderDiffuseDisk(colors, textureSize, galaxyColor,
                    cfg.DiffuseDiskBrightness, cfg.DiffuseDiskAlpha, cfg.DiffuseDiskExtent);
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var sector in sectors)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    RenderStars(sector.ClusterPoints, sector.CenterX, sector.CenterZ,
                        diskRadius, galaxyColor, colors, textureSize,
                        cfg.TextureBrightnessScale, cfg.TextureStarAlpha, cfg.TextureStarRadius);
                }
                cancellationToken.ThrowIfCancellationRequested();

                ApplyBlur(colors, textureSize, cfg.TextureBlurRadius + extraBlurRadius);
                cancellationToken.ThrowIfCancellationRequested();

                RenderBulge(colors, textureSize, galaxyColor,
                    cfg.BulgeExtent, cfg.BulgeIntensity, cfg.BulgeAlphaScale);
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

        private static void RenderDiffuseDisk(
            Color[,] colors, int textureSize, Color galaxyColor,
            float brightness, int alpha, float extent)
        {
            if (brightness <= 0 || alpha <= 0) return;

            var center = textureSize / 2f;
            var maxDist = center * extent;
            var tint = galaxyColor.ToVector3();

            for (int x = 0; x < textureSize; x++)
            for (int y = 0; y < textureSize; y++)
            {
                var dx = x - center;
                var dy = y - center;
                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                var intensity = Math.Max(0f, 1f - dist / maxDist);
                if (intensity <= 0) continue;

                var fade = intensity * intensity;
                var diskColor = tint * fade * brightness;
                var dc = new Color(diskColor);
                var a = (int)(fade * alpha);

                var c = colors[x, y];
                colors[x, y] = new Color(
                    Math.Min(c.R + dc.R, 255),
                    Math.Min(c.G + dc.G, 255),
                    Math.Min(c.B + dc.B, 255),
                    Math.Min(c.A + a, 255));
            }
        }

        private static void RenderStars(
            GalaxyClusterPoint[] stars, float offsetX, float offsetZ,
            float diskRadius, Color galaxyColor,
            Color[,] colors, int textureSize,
            float brightnessScale, int starAlpha, int starRadius)
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

                if (px < -starRadius || px >= textureSize + starRadius ||
                    py < -starRadius || py >= textureSize + starRadius)
                    continue;

                var distFromCenter = (float)Math.Sqrt(sx * sx + sz * sz) / (diskRadius * 0.9f);
                var brightness = MathHelper.Clamp(1f - distFromCenter * 0.6f, 0.2f, 1f) * brightnessScale;

                var starColorVec = GalaxyAsPointAggregatedData.ColorFromTemperature(star.Temperature).ToVector3();
                var blended = Vector3.Lerp(starColorVec, galaxyTint, 0.3f) * brightness;
                var starColor = new Color(blended);

                for (int dy = -starRadius; dy <= starRadius; dy++)
                for (int dx = -starRadius; dx <= starRadius; dx++)
                {
                    var tx = px + dx;
                    var ty = py + dy;
                    if (tx < 0 || tx >= textureSize || ty < 0 || ty >= textureSize)
                        continue;

                    var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist > starRadius) continue;
                    var falloff = 1f - dist / (starRadius + 1f);

                    var fr = (int)(starColor.R * falloff);
                    var fg = (int)(starColor.G * falloff);
                    var fb = (int)(starColor.B * falloff);
                    var fa = (int)(starAlpha * falloff);

                    var existing = colors[tx, ty];
                    colors[tx, ty] = new Color(
                        Math.Min(existing.R + fr, 255),
                        Math.Min(existing.G + fg, 255),
                        Math.Min(existing.B + fb, 255),
                        Math.Min(existing.A + fa, 255));
                }
            }
        }

        private static void ApplyBlur(Color[,] colors, int textureSize, int blurRadius)
        {
            if (blurRadius <= 0) return;

            var kernelSize = blurRadius * 2 + 1;
            var kernelArea = kernelSize * kernelSize;
            var temp = new Color[textureSize, textureSize];

            // Horizontal pass
            for (int y = 0; y < textureSize; y++)
            for (int x = 0; x < textureSize; x++)
            {
                int rr = 0, gg = 0, bb = 0, aa = 0, count = 0;
                for (int k = -blurRadius; k <= blurRadius; k++)
                {
                    var sx = x + k;
                    if (sx < 0 || sx >= textureSize) continue;
                    var c = colors[sx, y];
                    rr += c.R; gg += c.G; bb += c.B; aa += c.A;
                    count++;
                }
                temp[x, y] = new Color(rr / count, gg / count, bb / count, aa / count);
            }

            // Vertical pass
            for (int y = 0; y < textureSize; y++)
            for (int x = 0; x < textureSize; x++)
            {
                int rr = 0, gg = 0, bb = 0, aa = 0, count = 0;
                for (int k = -blurRadius; k <= blurRadius; k++)
                {
                    var sy = y + k;
                    if (sy < 0 || sy >= textureSize) continue;
                    var c = temp[x, sy];
                    rr += c.R; gg += c.G; bb += c.B; aa += c.A;
                    count++;
                }
                colors[x, y] = new Color(rr / count, gg / count, bb / count, aa / count);
            }
        }

        private static void RenderBulge(
            Color[,] colors, int textureSize, Color galaxyColor,
            float bulgeExtent, float bulgeIntensity, int bulgeAlphaScale)
        {
            var center = textureSize / 2f;
            var galaxyTint = galaxyColor.ToVector3();
            var bulgeColor = Vector3.Lerp(ColorYellow, galaxyTint, 0.4f);
            var maxDist = center * bulgeExtent;

            for (int x = 0; x < textureSize; x++)
            for (int y = 0; y < textureSize; y++)
            {
                var dx = x - center;
                var dy = y - center;
                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                var bulge = Math.Max(0f, 1f - dist / maxDist);
                if (bulge > 0)
                {
                    var bc = new Color(bulgeColor * bulge * bulgeIntensity);
                    var c = colors[x, y];
                    colors[x, y] = new Color(
                        Math.Min(c.R + bc.R, 255),
                        Math.Min(c.G + bc.G, 255),
                        Math.Min(c.B + bc.B, 255),
                        Math.Min(c.A + (int)(bulge * bulgeAlphaScale), 255));
                }
            }
        }
    }
}
