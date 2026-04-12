using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Generation;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class StarSystemsBatchGeometry : MeshGeometryBase<VertexPositionColor>
    {
        private const int CircleSides = 12;
        private const float RebuildDotThreshold = 0.985f;

        private readonly GalaxyClusterPoint[] _clusterPoints;
        private readonly Matrix _galaxyRotation;
        private readonly Func<Vector3> _getCameraPosition;
        private readonly Vector3 _worldPosition;

        private readonly float _dotBaseRadius;
        private readonly float _dotRadiusScale;
        private readonly float _dotBaseBrightness;
        private readonly float _dotBrightnessScale;
        private readonly float _dotBrightnessMin;
        private readonly float _dotBrightnessMax;
        private readonly float _dotGlowAlphaScale;
        private readonly float _dotGlowRadiusScale;

        private VertexPositionColor[] _cachedVertices;
        private Array _cachedIndices;
        private int _cachedPrimitiveCount;
        private Vector3 _lastCameraDir;

        public StarSystemsBatchGeometry(GalaxyClusterPoint[] clusterPoints, float batchRadius,
            Matrix galaxyRotation, Func<Vector3> getCameraPosition, Vector3 worldPosition)
            : base(VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList)
        {
            _clusterPoints = clusterPoints;
            _galaxyRotation = galaxyRotation;
            _getCameraPosition = getCameraPosition;
            _worldPosition = worldPosition;

            var cfg = GalaxyConfig.Instance.StarSystemsBatch.Node;
            _dotBaseRadius = cfg.DotBaseRadius;
            _dotRadiusScale = cfg.DotRadiusScale;
            _dotBaseBrightness = cfg.DotBaseBrightness;
            _dotBrightnessScale = cfg.DotBrightnessScale;
            _dotBrightnessMin = cfg.DotBrightnessMin;
            _dotBrightnessMax = cfg.DotBrightnessMax;
            _dotGlowAlphaScale = cfg.DotGlowAlphaScale;
            _dotGlowRadiusScale = cfg.DotGlowRadiusScale;
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
            var cameraPos = _getCameraPosition();
            var cameraDir = cameraPos - _worldPosition;
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
            Rebuild(cameraPos);
        }

        private void Rebuild(Vector3 cameraPos)
        {
            if (_clusterPoints.Length == 0)
            {
                _cachedVertices = new[] { new VertexPositionColor(Vector3.Zero, Color.Transparent) };
                _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(new[] { 0, 0, 0 });
                _cachedPrimitiveCount = 1;
                return;
            }

            var vertsPerStar = 1 + CircleSides + CircleSides;
            var idxPerStar = CircleSides * 3 + CircleSides * 6;
            var verts = new List<VertexPositionColor>(_clusterPoints.Length * vertsPerStar);
            var idx = new List<int>(_clusterPoints.Length * idxPerStar);

            for (int i = 0; i < _clusterPoints.Length; i++)
            {
                var p = _clusterPoints[i];
                var center = Vector3.Transform(new Vector3(p.X, p.Y, p.Z), _galaxyRotation);

                var lookDir = cameraPos - (_worldPosition + center);
                if (lookDir.LengthSquared() < 0.001f)
                    lookDir = Vector3.Forward;
                lookDir.Normalize();

                var refUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
                var right = Vector3.Normalize(Vector3.Cross(refUp, lookDir));
                var up = Vector3.Cross(lookDir, right);

                var starColor = GalaxyAsPointAggregatedData.ColorFromTemperature(p.Temperature);
                var brightness = MathHelper.Clamp(_dotBaseBrightness + p.Luminosity * _dotBrightnessScale, _dotBrightnessMin, _dotBrightnessMax);
                var coreColor = new Color(
                    starColor.R / 255f * brightness,
                    starColor.G / 255f * brightness,
                    starColor.B / 255f * brightness);

                var glowAlpha = brightness * _dotGlowAlphaScale;
                var glowColor = new Color(
                    starColor.R / 255f * glowAlpha,
                    starColor.G / 255f * glowAlpha,
                    starColor.B / 255f * glowAlpha,
                    0f);

                var coreRadius = _dotBaseRadius + p.Luminosity * _dotRadiusScale;
                var glowRadius = coreRadius * _dotGlowRadiusScale;

                AddGlowStar(verts, idx, center, coreRadius, glowRadius, right, up, coreColor, glowColor);
            }

            _cachedVertices = verts.ToArray();
            _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(idx.ToArray());
            _cachedPrimitiveCount = idx.Count / 3;
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
