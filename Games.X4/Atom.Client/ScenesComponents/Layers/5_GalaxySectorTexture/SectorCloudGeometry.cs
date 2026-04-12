using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X4World.Objects;

namespace Atom.Client.Components
{
    public class SectorCloudGeometry : MeshGeometryBase<VertexPositionColorTexture>
    {
        private const float RebuildDotThreshold = 0.97f;

        private readonly GalaxyClusterPoint[] _points;
        private readonly Matrix _galaxyRotation;
        private readonly Func<Vector3> _getCameraPosition;
        private readonly Vector3 _worldPosition;
        private readonly Color _galaxyColor;
        private readonly float _sectorRadius;
        private readonly float _spriteRadius;
        private readonly float _spriteBrightness;
        private readonly float _yScale;
        private readonly float _galaxyTintBlend;

        private VertexPositionColorTexture[] _cachedVertices;
        private Array _cachedIndices;
        private int _cachedPrimitiveCount;
        private Vector3 _lastCameraDir;

        public SectorCloudGeometry(
            GalaxyClusterPoint[] points,
            Matrix galaxyRotation,
            Func<Vector3> getCameraPosition,
            Vector3 worldPosition,
            Color galaxyColor,
            float sectorRadius,
            float spriteRadius, float spriteBrightness,
            float yScale, float galaxyTintBlend)
            : base(VertexPositionColorTexture.VertexDeclaration, PrimitiveType.TriangleList)
        {
            _points = points;
            _galaxyRotation = galaxyRotation;
            _getCameraPosition = getCameraPosition;
            _worldPosition = worldPosition;
            _galaxyColor = galaxyColor;
            _sectorRadius = sectorRadius;
            _spriteRadius = spriteRadius;
            _spriteBrightness = spriteBrightness;
            _yScale = yScale;
            _galaxyTintBlend = galaxyTintBlend;
        }

        public override VertexPositionColorTexture[] GetVertices()
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
            if (_points.Length == 0)
            {
                _cachedVertices = new[] { new VertexPositionColorTexture(Vector3.Zero, Color.Transparent, Vector2.Zero) };
                _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(new[] { 0, 0, 0 });
                _cachedPrimitiveCount = 1;
                return;
            }

            var verts = new List<VertexPositionColorTexture>();
            var idx = new List<int>();
            var galaxyTint = _galaxyColor.ToVector3();
            for (int i = 0; i < _points.Length; i++)
            {
                var p = _points[i];
                var center = Vector3.Transform(new Vector3(p.X, p.Y * _yScale, p.Z), _galaxyRotation);
                var radius = _sectorRadius * _spriteRadius;

                var tempColor = GalaxyAsPointAggregatedData.ColorFromTemperature(p.Temperature).ToVector3();
                var blended = Vector3.Lerp(tempColor, galaxyTint, _galaxyTintBlend);
                var b = _spriteBrightness * MathHelper.Clamp(p.Luminosity, 0.1f, 1f);
                var color = new Color(blended * b);

                EmitBillboard(verts, idx, cameraPos, center, radius, color);
            }

            _cachedVertices = verts.ToArray();
            _cachedIndices = IndicesBuffersFactory.CreateIndicesArray(idx.ToArray());
            _cachedPrimitiveCount = idx.Count / 3;
        }

        private void EmitBillboard(
            List<VertexPositionColorTexture> verts, List<int> idx,
            Vector3 cameraPos, Vector3 center, float radius, Color color)
        {
            var lookDir = cameraPos - (_worldPosition + center);
            if (lookDir.LengthSquared() < 0.001f)
                lookDir = Vector3.Forward;
            lookDir.Normalize();

            var refUp = Math.Abs(Vector3.Dot(lookDir, Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            var right = Vector3.Normalize(Vector3.Cross(refUp, lookDir));
            var up = Vector3.Cross(lookDir, right);

            int baseIdx = verts.Count;
            verts.Add(new VertexPositionColorTexture(center - right * radius - up * radius, color, new Vector2(0, 0)));
            verts.Add(new VertexPositionColorTexture(center + right * radius - up * radius, color, new Vector2(1, 0)));
            verts.Add(new VertexPositionColorTexture(center + right * radius + up * radius, color, new Vector2(1, 1)));
            verts.Add(new VertexPositionColorTexture(center - right * radius + up * radius, color, new Vector2(0, 1)));

            idx.Add(baseIdx);
            idx.Add(baseIdx + 1);
            idx.Add(baseIdx + 2);
            idx.Add(baseIdx);
            idx.Add(baseIdx + 2);
            idx.Add(baseIdx + 3);
        }
    }
}
