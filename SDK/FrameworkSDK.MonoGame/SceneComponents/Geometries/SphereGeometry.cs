using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions.Collections;

namespace FrameworkSDK.MonoGame.SceneComponents.Geometries
{
    public class SphereGeometry : StaticMeshGeometry<VertexPositionNormalTexture>
    {
        public SphereGeometry(int divisionsCount = 2) : base(
            VertexPositionNormalTexture.VertexDeclaration,
            PrimitiveType.TriangleList,
            CreateVertices(divisionsCount, out var indices),
            indices, indices.Length / 3)
        {
            this.FillNormals();
            this.FillTextureCoordinatesSphere();
        }

        private static VertexPositionNormalTexture[] CreateVertices(int divisionsCount, out short[] indices)
        {
            var icosahedronVectors = new List<Vector3>(IcosahedronGeometry.CreateVectors());
            var icosahedronIndices = new List<short>(IcosahedronGeometry.CreateIndices());

            EnumerableExtended.Repeat(() => Subdivide(icosahedronVectors, icosahedronIndices), divisionsCount);

            indices = icosahedronIndices.ToArray();
            return icosahedronVectors.Select(v =>
            {
                v.Normalize();
                return new VertexPositionNormalTexture(v, Vector3.Zero, Vector2.Zero);
            }).ToArray();
        }

        private static void Subdivide(List<Vector3> vectors, List<short> indices)
        {
            var midpointIndices = new Dictionary<string, short>();

            var newIndices = new List<short>(indices.Count * 4);

            for (var i = 0; i < indices.Count - 2; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var m01 = GetMidpointIndex(midpointIndices, vectors, i0, i1);
                var m12 = GetMidpointIndex(midpointIndices, vectors, i1, i2);
                var m02 = GetMidpointIndex(midpointIndices, vectors, i2, i0);

                newIndices.AddRange(
                    new[] {
                        i0,m01,m02
                        ,
                        i1,m12,m01
                        ,
                        i2,m02,m12
                        ,
                        m02,m01,m12
                    }
                );

            }

            indices.Clear();
            indices.AddRange(newIndices);
        }
        
        private static short GetMidpointIndex(Dictionary<string, short> midpointIndices, List<Vector3> vertices, short i0, short i1)
        {
            var edgeKey = $"{Math.Min(i0, i1)}_{Math.Max(i0, i1)}";
            
            if (!midpointIndices.TryGetValue(edgeKey, out var midpointIndex))
            {
                var v0 = vertices[i0];
                var v1 = vertices[i1];

                var midpoint = (v0 + v1) / 2f;

                if (vertices.Contains(midpoint))
                    midpointIndex = (short)vertices.IndexOf(midpoint);
                else
                {
                    midpointIndex = (short)vertices.Count;
                    vertices.Add(midpoint);
                    midpointIndices.Add(edgeKey, midpointIndex);
                }
            }

            return midpointIndex;

        }
    }
}