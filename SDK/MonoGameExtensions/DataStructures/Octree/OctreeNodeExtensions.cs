using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace MonoGameExtensions.DataStructures
{
    public static class OctreeNodeExtensions
    {
        /// <summary>
        /// No checks that the point inside the node!!!
        /// </summary>
        public static IOctreeNode<TData> GetChildContainsPoint<TData>(this IOctreeNode<TData> node, Vector3 point)
        {
            var diff = point - node.Center;
            return node.Children.Map[new Point3D(Math.Sign(diff.X), Math.Sign(diff.Y), Math.Sign(diff.Z))];
        }

        public static bool ContainsPoint<TData>(this IOctreeNode<TData> node, Vector3 point)
        {
            return node.BoundingBox.Contains(point) == ContainmentType.Contains;
        }

        /// <summary>
        /// No checks that the point inside the node!!
        /// </summary>
        public static IOctreeNode<TData> GetLeafWithPoint<TData>(this IOctreeNode<TData> node, Vector3 point)
        {
            var currentNode = node;
            while (currentNode.ContainsPoint(point) && !currentNode.Children.IsEmpty)
            {
                currentNode = currentNode.GetChildContainsPoint(point);
            }
            return currentNode;
        }
        
        public static IEnumerable<IOctreeNode<TData>> EnumerateAllLeafs<TData>(this IOctreeNode<TData> node)
        {
            var queue = new Queue<IOctreeNode<TData>>();
            var currentNode = node;
            queue.Enqueue(currentNode);
            while (queue.Count > 0)
            {
                currentNode = queue.Dequeue();
                
                if (currentNode.Children.IsEmpty)
                    yield return currentNode;

                foreach (var child in currentNode.Children.Nodes)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public static IEnumerable<IOctreeNode<TData>> EnumerateLeafsInRangeAroundPoint<TData>(this IOctreeNode<TData> node, Vector3 point, float range)
        {
            var queue = new Queue<IOctreeNode<TData>>();
            var currentNode = node;
            queue.Enqueue(currentNode);
            
            while (queue.Count > 0)
            {
                currentNode = queue.Dequeue();
                
                if (currentNode.Children.IsEmpty)
                    yield return currentNode;

                foreach (var child in currentNode.Children.Nodes)
                {
                    if ((point - child.Center).Length() < range + child.Size / 2) 
                        queue.Enqueue(child);
                }
            }
        }

        public static float GetDistanceFromPoint<T, TData>(this T node, Vector3 point) where T : IOctreeNode<TData>
        {
            return Vector3.Distance(point, node.Center) - node.Size / 2;
        } 
    }
}