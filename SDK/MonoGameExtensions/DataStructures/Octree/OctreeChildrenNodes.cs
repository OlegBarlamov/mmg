using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NetExtensions.Geometry;

namespace MonoGameExtensions.DataStructures
{
    public class OctreeChildrenNodes<TData>
    {
        public static OctreeChildrenNodes<TData> Empty { get; } = new OctreeChildrenNodes<TData>();

        public bool IsEmpty => Nodes.Count == 0;
        
        /// <summary>
        /// x: -1, y: -1, z: -1
        /// </summary>
        public IOctreeNode<TData> LeftBottomBackward { get; }
        /// <summary>
        /// x: +1, y: -1, z: -1
        /// </summary>
        public IOctreeNode<TData> RightBottomBackward { get; }
        /// <summary>
        /// x: +1, y: -1, z: +1
        /// </summary>
        public IOctreeNode<TData> RightBottomForward { get; }
        /// <summary>
        /// x: -1, y: -1, z: +1 
        /// </summary>
        public IOctreeNode<TData> LeftBottomForward { get; }
        /// <summary>
        /// x: -1, y: +1, z: -1
        /// </summary>
        public IOctreeNode<TData> LeftTopBackward { get; }
        /// <summary>
        /// x: +1, y: +1, z: -1
        /// </summary>
        public IOctreeNode<TData> RightTopBackward { get; }
        /// <summary>
        /// x: +1, y: +1, z: +1
        /// </summary>
        public IOctreeNode<TData> RightTopForward { get; }
        /// <summary>
        /// x: -1, y: +1, z: +1 
        /// </summary>
        public IOctreeNode<TData> LeftTopForward { get; }

        public IReadOnlyList<IOctreeNode<TData>> Nodes { get; }
        public IReadOnlyDictionary<Point3D, IOctreeNode<TData>> Map { get; }

        public OctreeChildrenNodes(
            IOctreeNode<TData> leftBottomBackward,
            IOctreeNode<TData> rightBottomBackward,
            IOctreeNode<TData> rightBottomForward,
            IOctreeNode<TData> leftBottomForward,
            IOctreeNode<TData> leftTopBackward,
            IOctreeNode<TData> rightTopBackward,
            IOctreeNode<TData> rightTopForward,
            IOctreeNode<TData> leftTopForward)
        {
            LeftBottomBackward = leftBottomBackward;
            RightBottomBackward = rightBottomBackward;
            RightBottomForward = rightBottomForward;
            LeftBottomForward = leftBottomForward;
            LeftTopBackward = leftTopBackward;
            RightTopBackward = rightTopBackward;
            RightTopForward = rightTopForward;
            LeftTopForward = leftTopForward;
            Nodes = new[]
            {
                LeftBottomBackward,
                RightBottomBackward,
                RightBottomForward,
                LeftBottomForward,
                LeftTopBackward,
                RightTopBackward,
                RightTopForward,
                LeftTopForward,
            };
            Map = GenerateMap();
        }

        public OctreeChildrenNodes([NotNull] IReadOnlyList<IOctreeNode<TData>> nodesLeftBottomBackwardCounterClockwise)
        {
            if (nodesLeftBottomBackwardCounterClockwise == null) throw new ArgumentNullException(nameof(nodesLeftBottomBackwardCounterClockwise));
            LeftBottomBackward = nodesLeftBottomBackwardCounterClockwise[0];
            RightBottomBackward = nodesLeftBottomBackwardCounterClockwise[1];
            RightBottomForward = nodesLeftBottomBackwardCounterClockwise[2];
            LeftBottomForward = nodesLeftBottomBackwardCounterClockwise[3];
            LeftTopBackward = nodesLeftBottomBackwardCounterClockwise[4];
            RightTopBackward = nodesLeftBottomBackwardCounterClockwise[5];
            RightTopForward = nodesLeftBottomBackwardCounterClockwise[6];
            LeftTopForward = nodesLeftBottomBackwardCounterClockwise[7];
            Nodes = nodesLeftBottomBackwardCounterClockwise;
            Map = GenerateMap();
        }

        private OctreeChildrenNodes()
        {
            Nodes = new IOctreeNode<TData>[0];
        }

        private Dictionary<Point3D, IOctreeNode<TData>> GenerateMap()
        {
            return new Dictionary<Point3D, IOctreeNode<TData>>
            {
                {new Point3D(-1, -1, -1), LeftBottomBackward},
                {new Point3D(1, -1, -1), RightBottomBackward},
                {new Point3D(1, -1, 1), RightBottomForward},
                {new Point3D(-1, -1, 1), LeftBottomForward},
                {new Point3D(-1, 1, -1), LeftTopBackward},
                {new Point3D(1, 1, -1), RightTopBackward},
                {new Point3D(1, 1, 1), RightTopForward},
                {new Point3D(-1, 1, 1), LeftTopForward},
                
                // Edge cases
                {new Point3D(0, 0, 0), LeftBottomBackward},
                
                {new Point3D(-1, -1, 0), LeftBottomBackward},
                {new Point3D(0, -1, -1), LeftBottomBackward},
                {new Point3D(0, 0, -1), LeftBottomBackward},
                {new Point3D(0, -1, 0), LeftBottomBackward},
                {new Point3D(-1, 0, 0), LeftBottomBackward},

                {new Point3D(1, 1, 0), RightTopForward},
                {new Point3D(0, 1, 1), RightTopForward},
                {new Point3D(0, 0, 1), RightTopForward},
                {new Point3D(0, 1, 0), RightTopForward},
                {new Point3D(1, 0, 0), RightTopForward},
                
                {new Point3D(1, -1, 0), RightBottomForward},
                {new Point3D(0, -1, 1), LeftBottomForward},
                
                {new Point3D(-1, 1, 0), LeftTopBackward},
                {new Point3D(0, 1, -1), RightTopBackward},
            };
        }
    }
}