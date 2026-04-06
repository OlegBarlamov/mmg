using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxySectorChunkGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        private const int CircleSides = 12;

        public GalaxySectorChunkGeometry(GalaxyClusterPoint[] clusterPoints, float chunkRadius,
            Vector3 localCameraDirection, Matrix galaxyRotation)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(clusterPoints, chunkRadius, localCameraDirection, galaxyRotation, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            GalaxyClusterPoint[] clusterPoints, float chunkRadius,
            Vector3 lookDir, Matrix galaxyRotation, out int[] indices)
        {
            if (clusterPoints.Length == 0)
            {
                indices = new int[] { 0, 0, 0 };
                return new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
            }

            var vertsPerStar = 1 + CircleSides + CircleSides;
            var idxPerStar = CircleSides * 3 + CircleSides * 6;
            var verts = new List<VertexPositionColor>(clusterPoints.Length * vertsPerStar);
            var idx = new List<int>(clusterPoints.Length * idxPerStar);

            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var worldUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(worldUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            for (int i = 0; i < clusterPoints.Length; i++)
            {
                var p = clusterPoints[i];
                var center = Vector3.Transform(new Vector3(p.X, p.Y, p.Z), galaxyRotation);

                var starColor = GalaxyAsPointAggregatedData.ColorFromTemperature(p.Temperature);
                var brightness = MathHelper.Clamp(0.4f + p.Luminosity * 0.8f, 0.2f, 1f);
                var coreColor = new Color(
                    starColor.R / 255f * brightness,
                    starColor.G / 255f * brightness,
                    starColor.B / 255f * brightness);

                var glowAlpha = brightness * 0.3f;
                var glowColor = new Color(
                    starColor.R / 255f * glowAlpha,
                    starColor.G / 255f * glowAlpha,
                    starColor.B / 255f * glowAlpha,
                    0f);

                var coreRadius = chunkRadius * (0.006f + p.Luminosity * 0.012f);
                var glowRadius = coreRadius * 2f;

                AddGlowStar(verts, idx, center, coreRadius, glowRadius, right, up, coreColor, glowColor);
            }

            indices = idx.ToArray();
            return verts.ToArray();
        }

        private static void AddGlowStar(
            List<VertexPositionColor> verts, List<int> idx,
            Vector3 center, float coreRadius, float glowRadius,
            Vector3 right, Vector3 up,
            Color coreColor, Color glowColor)
        {
            int centerIdx = verts.Count;
            verts.Add(new VertexPositionColor(center, coreColor));

            for (int s = 0; s < CircleSides; s++)
            {
                var angle = s * MathHelper.TwoPi / CircleSides;
                var dir = right * (float)Math.Cos(angle) + up * (float)Math.Sin(angle);
                verts.Add(new VertexPositionColor(center + dir * coreRadius, coreColor));
            }

            for (int s = 0; s < CircleSides; s++)
            {
                var angle = s * MathHelper.TwoPi / CircleSides;
                var dir = right * (float)Math.Cos(angle) + up * (float)Math.Sin(angle);
                verts.Add(new VertexPositionColor(center + dir * glowRadius, glowColor));
            }

            for (int s = 0; s < CircleSides; s++)
            {
                var next = (s + 1) % CircleSides;
                idx.Add(centerIdx);
                idx.Add(centerIdx + 1 + next);
                idx.Add(centerIdx + 1 + s);
            }

            for (int s = 0; s < CircleSides; s++)
            {
                var next = (s + 1) % CircleSides;
                var innerCur = centerIdx + 1 + s;
                var innerNext = centerIdx + 1 + next;
                var outerCur = centerIdx + 1 + CircleSides + s;
                var outerNext = centerIdx + 1 + CircleSides + next;

                idx.Add(innerCur);
                idx.Add(outerNext);
                idx.Add(outerCur);

                idx.Add(innerCur);
                idx.Add(innerNext);
                idx.Add(outerNext);
            }
        }
    }
}
