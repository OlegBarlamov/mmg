using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Generation;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemAsPointGeometry : MeshGeometryBase<VertexPositionColor>
    {
        private const int HexSides = 6;
        private const float RebuildDotThreshold = 0.985f;

        private readonly Func<Vector3> _getCameraPosition;
        private readonly Vector3 _worldPosition;
        private readonly Color _starColor;
        private readonly Color _edgeColor;
        private readonly float _radius;

        private VertexPositionColor[] _cachedVertices;
        private Array _cachedIndices;
        private int _cachedPrimitiveCount;
        private Vector3 _lastLookDir;

        public StarSystemAsPointGeometry(
            StarSystemAsPointAggregatedData data,
            Vector3 worldPosition,
            Func<Vector3> getCameraPosition)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList)
        {
            _worldPosition = worldPosition;
            _getCameraPosition = getCameraPosition;

            var cfg = GalaxyConfig.Instance.StarSystemAsPoint.Node;
            var baseColor = GalaxyAsPointAggregatedData.ColorFromTemperature(data.Temperature);
            var brightness = cfg.DotBaseBrightness + data.Luminosity * cfg.DotBrightnessScale;

            _starColor = new Color(
                baseColor.R / 255f * brightness,
                baseColor.G / 255f * brightness,
                baseColor.B / 255f * brightness);

            _edgeColor = new Color(
                _starColor.R / 255f * cfg.DotEdgeBrightness,
                _starColor.G / 255f * cfg.DotEdgeBrightness,
                _starColor.B / 255f * cfg.DotEdgeBrightness);

            _radius = cfg.DotBaseRadius + data.Luminosity * cfg.DotRadiusScale;
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
            var lookDir = _getCameraPosition() - _worldPosition;
            if (_cachedVertices != null)
            {
                var len = lookDir.LengthSquared();
                if (len > 0.0001f)
                {
                    var newNorm = lookDir * (1f / (float)Math.Sqrt(len));
                    var oldLen = _lastLookDir.LengthSquared();
                    if (oldLen > 0.0001f)
                    {
                        var oldNorm = _lastLookDir * (1f / (float)Math.Sqrt(oldLen));
                        if (Vector3.Dot(newNorm, oldNorm) > RebuildDotThreshold)
                            return;
                    }
                }
            }

            _lastLookDir = lookDir;
            Rebuild(lookDir);
        }

        private void Rebuild(Vector3 lookDir)
        {
            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var refUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(refUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            var verts = new List<VertexPositionColor>(HexSides + 1);
            var idx = new List<int>(HexSides * 3);

            SectorStarFieldGeometry.AddHexStar(verts, idx, Vector3.Zero, _radius, right, up, _starColor, _edgeColor);

            _cachedVertices = verts.ToArray();
            _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(idx.ToArray());
            _cachedPrimitiveCount = idx.Count / 3;
        }
    }
}
