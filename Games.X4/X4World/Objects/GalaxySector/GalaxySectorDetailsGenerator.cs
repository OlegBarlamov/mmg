using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorDetailsGenerator : IDetailsGenerator<GalaxySector>
    {
        private const int MaxPointsPerChunk = 150;
        private const int MaxSplitDepth = 5;

        public void Generate(GalaxySector target)
        {
            var aggData = target.AggregatedData;
            var clusterPoints = aggData.ClusterPoints;
            var sectorRadius = aggData.SectorRadius;

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var results = new List<IWrappedDetails>();
            AdaptiveSplit(target, clusterPoints,
                -sectorRadius, -sectorRadius, sectorRadius * 2f, sectorRadius * 2f,
                0, aggData, galaxyRotation, results);

            target.SetGeneratedData(results);
        }

        private static void AdaptiveSplit(GalaxySector parent, GalaxyClusterPoint[] points,
            float minX, float minZ, float sizeX, float sizeZ,
            int depth,
            GalaxySectorAggregatedData aggData, Matrix galaxyRotation,
            List<IWrappedDetails> result)
        {
            if (points.Length == 0)
                return;

            if (points.Length <= MaxPointsPerChunk || depth >= MaxSplitDepth)
            {
                EmitChunk(parent, points, minX, minZ, sizeX, sizeZ, aggData, galaxyRotation, result);
                return;
            }

            var halfX = sizeX * 0.5f;
            var halfZ = sizeZ * 0.5f;
            var midX = minX + halfX;
            var midZ = minZ + halfZ;

            var q0 = new List<GalaxyClusterPoint>();
            var q1 = new List<GalaxyClusterPoint>();
            var q2 = new List<GalaxyClusterPoint>();
            var q3 = new List<GalaxyClusterPoint>();

            foreach (var p in points)
            {
                if (p.X < midX)
                {
                    if (p.Z < midZ) q0.Add(p); else q2.Add(p);
                }
                else
                {
                    if (p.Z < midZ) q1.Add(p); else q3.Add(p);
                }
            }

            AdaptiveSplit(parent, q0.ToArray(), minX, minZ, halfX, halfZ, depth + 1, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q1.ToArray(), midX, minZ, halfX, halfZ, depth + 1, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q2.ToArray(), minX, midZ, halfX, halfZ, depth + 1, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q3.ToArray(), midX, midZ, halfX, halfZ, depth + 1, aggData, galaxyRotation, result);
        }

        private static void EmitChunk(GalaxySector parent, GalaxyClusterPoint[] points,
            float minX, float minZ, float sizeX, float sizeZ,
            GalaxySectorAggregatedData aggData, Matrix galaxyRotation,
            List<IWrappedDetails> result)
        {
            var cx = minX + sizeX * 0.5f;
            var cz = minZ + sizeZ * 0.5f;
            var chunkRadius = Math.Max(sizeX, sizeZ) * 0.5f;

            var localPoints = new GalaxyClusterPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                var rp = points[i];
                localPoints[i] = new GalaxyClusterPoint(rp.X - cx, rp.Y, rp.Z - cz, rp.Temperature, rp.Luminosity);
            }

            var chunkPos = Vector3.Transform(new Vector3(cx, 0f, cz), galaxyRotation);
            var chunkData = new GalaxySectorChunkAggregatedData(
                chunkRadius, aggData.Inclination, aggData.SpinAngle, localPoints);

            result.Add(new GalaxySectorChunk(parent, chunkPos, chunkRadius * 3f, chunkData));
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySector)target);
        }
    }
}
