using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorChunkDetailsGenerator : IDetailsGenerator<GalaxySectorChunk>
    {

        public void Generate(GalaxySectorChunk target)
        {
            var cfg = GalaxyConfig.Instance.GalaxySectorChunk.Generation;
            var aggData = target.AggregatedData;
            var chunkRadius = aggData.ChunkRadius;

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var results = new List<IWrappedDetails>();

            AdaptiveSplit(target, aggData.ClusterPoints,
                -chunkRadius, -chunkRadius, chunkRadius * 2f, chunkRadius * 2f,
                0, cfg.MaxPointsPerBatch, cfg.MaxSplitDepth, aggData, galaxyRotation, results);

            target.SetGeneratedData(results);
        }

        private static void AdaptiveSplit(GalaxySectorChunk parent, GalaxyClusterPoint[] points,
            float minX, float minZ, float sizeX, float sizeZ,
            int depth, int maxPointsPerBatch, int maxSplitDepth,
            GalaxySectorChunkAggregatedData aggData, Matrix galaxyRotation,
            List<IWrappedDetails> result)
        {
            if (points.Length == 0)
                return;

            if (points.Length <= maxPointsPerBatch || depth >= maxSplitDepth)
            {
                EmitBatch(parent, points, minX, minZ, sizeX, sizeZ, aggData, galaxyRotation, result);
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

            AdaptiveSplit(parent, q0.ToArray(), minX, minZ, halfX, halfZ, depth + 1, maxPointsPerBatch, maxSplitDepth, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q1.ToArray(), midX, minZ, halfX, halfZ, depth + 1, maxPointsPerBatch, maxSplitDepth, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q2.ToArray(), minX, midZ, halfX, halfZ, depth + 1, maxPointsPerBatch, maxSplitDepth, aggData, galaxyRotation, result);
            AdaptiveSplit(parent, q3.ToArray(), midX, midZ, halfX, halfZ, depth + 1, maxPointsPerBatch, maxSplitDepth, aggData, galaxyRotation, result);
        }

        private static void EmitBatch(GalaxySectorChunk parent, GalaxyClusterPoint[] points,
            float minX, float minZ, float sizeX, float sizeZ,
            GalaxySectorChunkAggregatedData aggData, Matrix galaxyRotation,
            List<IWrappedDetails> result)
        {
            var cx = minX + sizeX * 0.5f;
            var cz = minZ + sizeZ * 0.5f;

            var cellRadius = Math.Max(sizeX, sizeZ) * 0.5f;
            var localPoints = new GalaxyClusterPoint[points.Length];
            float maxExtent = cellRadius;
            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                localPoints[i] = new GalaxyClusterPoint(p.X - cx, p.Y, p.Z - cz, p.Temperature, p.Luminosity);
                var v = Vector3.Transform(new Vector3(p.X - cx, p.Y, p.Z - cz), galaxyRotation);
                maxExtent = Math.Max(maxExtent, Math.Max(Math.Abs(v.X), Math.Max(Math.Abs(v.Y), Math.Abs(v.Z))));
            }

            var batchPos = Vector3.Transform(new Vector3(cx, 0f, cz), galaxyRotation);
            var batchData = new StarSystemsBatchAggregatedData(
                maxExtent, cellRadius, aggData.SectorRadius, aggData.Inclination, aggData.SpinAngle, localPoints);

            result.Add(new StarSystemsBatch(parent, batchPos, batchData));
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorChunk)target);
        }
    }
}
