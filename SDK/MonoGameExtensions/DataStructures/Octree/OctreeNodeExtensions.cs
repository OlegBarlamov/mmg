using System;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace MonoGameExtensions.DataStructures
{
    public static class OctreeNodeExtensions
    {
        /// <summary>
        /// No checks!!! 
        /// </summary>
        public static T GetChildContainsPoint<T, TData>(this T node, Vector3 point) where T : IOctreeNode<TData>
        {
            var diff = point - node.Center;
            return (T)node.Children.Map[new Point3D(Math.Sign(diff.X), Math.Sign(diff.Y), Math.Sign(diff.Z))];
        }
    }
}