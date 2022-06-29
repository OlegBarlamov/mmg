using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Graphics.Meshes
{
    public static class MeshGeometriesHelpers
    {
        public static void FillNormals(this MeshGeometryBase<VertexPositionNormalTexture> geometry)
        {
            var vertices = geometry.GetVertices();
            var indices = geometry.GetIndices();
            if (indices.Length < 1)
                return;
            
            bool isShort = indices.GetValue(0) is short;

            for (int i = 0; i < indices.Length; i += 3)
            {
                var index0 = GetIntFromIndicesArray(indices, i, isShort);
                var index1 = GetIntFromIndicesArray(indices, i + 1, isShort);
                var index2 = GetIntFromIndicesArray(indices, i + 2, isShort);

                var vertex0 = vertices[index0];
                var vertex1 = vertices[index1];
                var vertex2 = vertices[index2];
                
                var normal = GetNormal(vertex0, vertex1, vertex2);
                
                var normal0 = vertices[index0].Normal + normal;
                var normal1 = vertices[index1].Normal + normal;
                var normal2 = vertices[index2].Normal + normal;
                
                vertices[index0] = new VertexPositionNormalTexture(vertex0.Position, normal0, vertex0.TextureCoordinate);
                vertices[index1] = new VertexPositionNormalTexture(vertex1.Position, normal1, vertex1.TextureCoordinate);
                vertices[index2] = new VertexPositionNormalTexture(vertex2.Position, normal2, vertex2.TextureCoordinate);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }
        
        private static int GetIntFromIndicesArray(Array indicesArray, int index, bool isShortType)
        {
            return isShortType ? (short) indicesArray.GetValue(index) : (int) indicesArray.GetValue(index);
        }

        public static Vector3 GetNormal(VertexPositionNormalTexture v1, VertexPositionNormalTexture v2,
            VertexPositionNormalTexture v3)
        {
            return GetNormal(v1.Position, v2.Position, v3.Position);
        }

        public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(b - a, c - b);
        }

        public static void FillTextureCoordinatesSphere(this MeshGeometryBase<VertexPositionNormalTexture> geometry)
        {
            var vertices = geometry.GetVertices();
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertices[i].TextureCoordinate = WorldToSpherical(vertex.Position);
            }
        }
        
        private static Vector2 WorldToSpherical(Vector3 p) {   

            var radius = Math.Sqrt(p.X*p.X + p.Y*p.Y + p.Z*p.Z);
            var longitudeRadians = Math.Atan2(p.Y, p.X);
            var latitudeRadians = Math.Asin(p.Z/radius);

            // Convert range -PI...PI to 0...1
            var s =  longitudeRadians/(2 * Math.PI) + 0.5;

            // Convert range -PI/2...PI/2 to 0...1
            var t =  latitudeRadians/Math.PI + 0.5;  
            return new Vector2((float)s, (float)t);

        }
    }
}