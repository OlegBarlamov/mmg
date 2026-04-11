using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class SectorStarFieldGeometry : MeshGeometryBase<VertexPositionColor>
    {
        private const int HexSides = 6;
        private const float RebuildDotThreshold = 0.985f;

        private readonly GalaxyClusterPoint[] _clusterPoints;
        private readonly Matrix _galaxyRotation;
        private readonly Func<Vector3> _getCameraPosition;
        private readonly Vector3 _worldPosition;

        private VertexPositionColor[] _cachedVertices;
        private Array _cachedIndices;
        private int _cachedPrimitiveCount;
        private Vector3 _lastCameraDir;

        public SectorStarFieldGeometry(GalaxyClusterPoint[] clusterPoints, float sectorRadius,
            Matrix galaxyRotation, Func<Vector3> getCameraPosition, Vector3 worldPosition)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList)
        {
            _clusterPoints = clusterPoints;
            _galaxyRotation = galaxyRotation;
            _getCameraPosition = getCameraPosition;
            _worldPosition = worldPosition;
        }

        public override VertexPositionColor[] GetVertices()
        {
            EnsureUpToDate();
            return _cachedVertices;
        }

        public override Array GetIndices()
        {
            EnsureUpToDate();
            return _cachedIndices;
        }

        public override int GetPrimitivesCount()
        {
            EnsureUpToDate();
            return _cachedPrimitiveCount;
        }

        private void EnsureUpToDate()
        {
            var cameraDir = _getCameraPosition() - _worldPosition;
            if (_cachedVertices != null)
            {
                var len = cameraDir.LengthSquared();
                if (len > 0.0001f)
                {
                    var newNorm = cameraDir * (1f / (float)Math.Sqrt(len));
                    var oldLen = _lastCameraDir.LengthSquared();
                    if (oldLen > 0.0001f)
                    {
                        var oldNorm = _lastCameraDir * (1f / (float)Math.Sqrt(oldLen));
                        if (Vector3.Dot(newNorm, oldNorm) > RebuildDotThreshold)
                            return;
                    }
                }
            }

            _lastCameraDir = cameraDir;
            Rebuild(cameraDir);
        }

        private void Rebuild(Vector3 lookDir)
        {
            if (_clusterPoints.Length == 0)
            {
                _cachedVertices = new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
                _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(new[] { 0, 0, 0 });
                _cachedPrimitiveCount = 1;
                return;
            }

            var verts = new List<VertexPositionColor>(_clusterPoints.Length * (HexSides + 1));
            var idx = new List<int>(_clusterPoints.Length * HexSides * 3);

            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var worldUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(worldUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            for (int i = 0; i < _clusterPoints.Length; i++)
            {
                var p = _clusterPoints[i];
                var center = Vector3.Transform(new Vector3(p.X, p.Y, p.Z), _galaxyRotation);

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

                var radius = 0.03f + p.Luminosity * 0.05f;

                AddHexStar(verts, idx, center, radius, right, up, starColor, edgeColor);
            }

            _cachedVertices = verts.ToArray();
            _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(idx.ToArray());
            _cachedPrimitiveCount = idx.Count / 3;
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
