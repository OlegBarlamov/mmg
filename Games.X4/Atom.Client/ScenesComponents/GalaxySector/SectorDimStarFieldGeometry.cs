using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class SectorDimStarFieldGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public SectorDimStarFieldGeometry(float sectorRadius, int starCount, int seed, Vector3 localCameraDirection)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(sectorRadius, starCount, seed, localCameraDirection, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            float sectorRadius, int starCount, int seed, Vector3 lookDir, out short[] indices)
        {
            var rng = new Random(seed);
            var verts = new List<VertexPositionColor>(starCount * 7);
            var idx = new List<short>(starCount * 18);

            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var worldUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(worldUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            for (int i = 0; i < starCount; i++)
            {
                var cx = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius;
                var cy = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.1f;
                var cz = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius;
                var center = new Vector3(cx, cy, cz);

                var temperature = 1000f + (float)rng.NextDouble() * 9000f;
                var luminosity = 0.05f + (float)rng.NextDouble() * 0.25f;

                var baseColor = GalaxyAsPointAggregatedData.ColorFromTemperature(temperature);
                var starColor = new Color(
                    baseColor.R / 255f * luminosity,
                    baseColor.G / 255f * luminosity,
                    baseColor.B / 255f * luminosity);

                var edgeColor = new Color(
                    baseColor.R / 255f * luminosity * 0.1f,
                    baseColor.G / 255f * luminosity * 0.1f,
                    baseColor.B / 255f * luminosity * 0.1f);

                var radius = sectorRadius * (0.01f + luminosity * 0.02f);

                SectorStarFieldGeometry.AddHexStar(verts, idx, center, radius, right, up, starColor, edgeColor);
            }

            indices = idx.ToArray();
            return verts.ToArray();
        }
    }
}
