using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class SectorStarFieldGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        private const int PreviewStarCount = 30;
        private const int HexSides = 6;

        public SectorStarFieldGeometry(float sectorRadius, int seed, Vector3 localCameraDirection)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(sectorRadius, seed, localCameraDirection, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            float sectorRadius, int seed, Vector3 lookDir, out short[] indices)
        {
            var rng = new Random(seed);
            var verts = new List<VertexPositionColor>(PreviewStarCount * (HexSides + 1));
            var idx = new List<short>(PreviewStarCount * HexSides * 3);

            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var worldUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(worldUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            for (int i = 0; i < PreviewStarCount; i++)
            {
                var cx = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius;
                var cy = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.1f;
                var cz = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius;
                var center = new Vector3(cx, cy, cz);

                var temperature = 1000f + (float)rng.NextDouble() * 9000f;
                var luminosity = (float)rng.NextDouble();
                rng.Next(); // seed

                var starColor = GalaxyAsPointAggregatedData.ColorFromTemperature(temperature);
                var brightness = 0.5f + luminosity * 0.5f;
                starColor = new Color(
                    starColor.R / 255f * brightness,
                    starColor.G / 255f * brightness,
                    starColor.B / 255f * brightness);

                var edgeColor = new Color(
                    starColor.R / 255f * 0.15f,
                    starColor.G / 255f * 0.15f,
                    starColor.B / 255f * 0.15f);

                var radius = sectorRadius * (0.01f + luminosity * 0.015f);

                AddHexStar(verts, idx, center, radius, right, up, starColor, edgeColor);
            }

            indices = idx.ToArray();
            return verts.ToArray();
        }

        internal static void AddHexStar(
            List<VertexPositionColor> verts, List<short> idx,
            Vector3 center, float radius, Vector3 right, Vector3 up,
            Color centerColor, Color edgeColor)
        {
            short centerIdx = (short)verts.Count;
            verts.Add(new VertexPositionColor(center, centerColor));

            for (int s = 0; s < HexSides; s++)
            {
                var angle = s * MathHelper.TwoPi / HexSides;
                var offset = right * ((float)Math.Cos(angle) * radius)
                           + up * ((float)Math.Sin(angle) * radius);
                verts.Add(new VertexPositionColor(center + offset, edgeColor));
            }

            for (int s = 0; s < HexSides; s++)
            {
                var next = (s + 1) % HexSides;
                idx.Add(centerIdx);
                idx.Add((short)(centerIdx + 1 + next));
                idx.Add((short)(centerIdx + 1 + s));
            }
        }
    }
}
