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
        private const int HexSides = 6;

        public SectorStarFieldGeometry(GalaxyClusterPoint[] clusterPoints, float sectorRadius,
            Vector3 localCameraDirection, Matrix galaxyRotation)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(clusterPoints, sectorRadius, localCameraDirection, galaxyRotation, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            GalaxyClusterPoint[] clusterPoints, float sectorRadius,
            Vector3 lookDir, Matrix galaxyRotation, out int[] indices)
        {
            if (clusterPoints.Length == 0)
            {
                indices = new int[] { 0, 0, 0 };
                return new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
            }

            var verts = new List<VertexPositionColor>(clusterPoints.Length * (HexSides + 1));
            var idx = new List<int>(clusterPoints.Length * HexSides * 3);

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
                var brightness = 0.3f + p.Luminosity * 0.7f;
                starColor = new Color(
                    starColor.R / 255f * brightness,
                    starColor.G / 255f * brightness,
                    starColor.B / 255f * brightness);

                var edgeColor = new Color(
                    starColor.R / 255f * 0.15f,
                    starColor.G / 255f * 0.15f,
                    starColor.B / 255f * 0.15f);

                var radius = sectorRadius * (0.003f + p.Luminosity * 0.007f);

                AddHexStar(verts, idx, center, radius, right, up, starColor, edgeColor);
            }

            indices = idx.ToArray();
            return verts.ToArray();
        }

        internal static void AddHexStar(
            List<VertexPositionColor> verts, List<int> idx,
            Vector3 center, float radius, Vector3 right, Vector3 up,
            Color centerColor, Color edgeColor)
        {
            int centerIdx = verts.Count;
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
                idx.Add(centerIdx + 1 + next);
                idx.Add(centerIdx + 1 + s);
            }
        }
    }
}
