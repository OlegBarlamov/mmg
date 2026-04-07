using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemsBatchGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        public StarSystemsBatchGeometry(GalaxyClusterPoint[] clusterPoints, float batchRadius,
            Vector3 localCameraDirection, Matrix galaxyRotation)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(clusterPoints, batchRadius, localCameraDirection, galaxyRotation, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            GalaxyClusterPoint[] clusterPoints, float batchRadius,
            Vector3 lookDir, Matrix galaxyRotation, out int[] indices)
        {
            if (clusterPoints.Length == 0)
            {
                indices = new int[] { 0, 0, 0 };
                return new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
            }

            const int vertsPerStar = 5;
            const int idxPerStar = 12;
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

                var edgeAlpha = brightness * 0.15f;
                var edgeColor = new Color(
                    starColor.R / 255f * edgeAlpha,
                    starColor.G / 255f * edgeAlpha,
                    starColor.B / 255f * edgeAlpha,
                    0f);

                var halfSize = batchRadius * (0.008f + p.Luminosity * 0.016f);

                AddBillboardQuad(verts, idx, center, halfSize, right, up, coreColor, edgeColor);
            }

            indices = idx.ToArray();
            return verts.ToArray();
        }

        private static void AddBillboardQuad(
            List<VertexPositionColor> verts, List<int> idx,
            Vector3 center, float halfSize,
            Vector3 right, Vector3 up,
            Color coreColor, Color edgeColor)
        {
            int centerIdx = verts.Count;

            verts.Add(new VertexPositionColor(center, coreColor));
            verts.Add(new VertexPositionColor(center + (-right + up) * halfSize, edgeColor));
            verts.Add(new VertexPositionColor(center + (right + up) * halfSize, edgeColor));
            verts.Add(new VertexPositionColor(center + (right - up) * halfSize, edgeColor));
            verts.Add(new VertexPositionColor(center + (-right - up) * halfSize, edgeColor));

            idx.Add(centerIdx); idx.Add(centerIdx + 1); idx.Add(centerIdx + 2);
            idx.Add(centerIdx); idx.Add(centerIdx + 2); idx.Add(centerIdx + 3);
            idx.Add(centerIdx); idx.Add(centerIdx + 3); idx.Add(centerIdx + 4);
            idx.Add(centerIdx); idx.Add(centerIdx + 4); idx.Add(centerIdx + 1);
        }
    }
}
