using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class GalaxiesBatchGeometry : StaticMeshGeometry<VertexPositionColor>
    {
        private const float BaseGalaxyHalfSize = 30f;

        public GalaxiesBatchGeometry(WorldMapCellAggregatedData.GalaxyPointData[] galaxyPoints,
            Vector3 localCameraDirection)
            : base(
                VertexPositionColor.VertexDeclaration,
                PrimitiveType.TriangleList,
                CreateVertices(galaxyPoints, localCameraDirection, out var indices),
                indices,
                indices.Length / 3)
        {
        }

        private static VertexPositionColor[] CreateVertices(
            WorldMapCellAggregatedData.GalaxyPointData[] galaxyPoints,
            Vector3 lookDir, out int[] indices)
        {
            if (galaxyPoints.Length == 0)
            {
                indices = new int[] { 0, 0, 0 };
                return new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
            }

            const int vertsPerGalaxy = 5;
            const int idxPerGalaxy = 12;
            var verts = new List<VertexPositionColor>(galaxyPoints.Length * vertsPerGalaxy);
            var idx = new List<int>(galaxyPoints.Length * idxPerGalaxy);

            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var worldUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(worldUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            for (int i = 0; i < galaxyPoints.Length; i++)
            {
                var gp = galaxyPoints[i];
                var center = gp.LocalPositionFromCenter;

                var starColor = GalaxyAsPointAggregatedData.ColorFromTemperature(gp.Temperature);
                var coreColor = new Color(starColor.R / 255f, starColor.G / 255f, starColor.B / 255f);

                var edgeColor = new Color(
                    starColor.R / 255f * 0.15f,
                    starColor.G / 255f * 0.15f,
                    starColor.B / 255f * 0.15f,
                    0f);

                var halfSize = BaseGalaxyHalfSize * gp.Power;
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
