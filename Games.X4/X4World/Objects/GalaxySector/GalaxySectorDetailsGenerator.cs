using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorDetailsGenerator : IDetailsGenerator<GalaxySector>
    {
        private const int ChunkGridSize = 3;

        public void Generate(GalaxySector target)
        {
            var aggData = target.AggregatedData;
            var clusterPoints = aggData.ClusterPoints;
            var sectorRadius = aggData.SectorRadius;

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var chunkCellSize = sectorRadius * 2f / ChunkGridSize;
            var chunkRadius = chunkCellSize * 0.5f;

            var buckets = new Dictionary<int, List<GalaxyClusterPoint>>();
            foreach (var p in clusterPoints)
            {
                var col = (int)((p.X + sectorRadius) / chunkCellSize);
                var row = (int)((p.Z + sectorRadius) / chunkCellSize);
                col = Math.Max(0, Math.Min(col, ChunkGridSize - 1));
                row = Math.Max(0, Math.Min(row, ChunkGridSize - 1));

                var key = row * ChunkGridSize + col;
                if (!buckets.TryGetValue(key, out var list))
                {
                    list = new List<GalaxyClusterPoint>();
                    buckets[key] = list;
                }
                list.Add(p);
            }

            var results = new List<IWrappedDetails>(buckets.Count);
            foreach (var kvp in buckets)
            {
                var row = kvp.Key / ChunkGridSize;
                var col = kvp.Key % ChunkGridSize;
                var cx = -sectorRadius + (col + 0.5f) * chunkCellSize;
                var cz = -sectorRadius + (row + 0.5f) * chunkCellSize;

                var rawPoints = kvp.Value;
                var localPoints = new GalaxyClusterPoint[rawPoints.Count];
                for (int i = 0; i < rawPoints.Count; i++)
                {
                    var rp = rawPoints[i];
                    localPoints[i] = new GalaxyClusterPoint(rp.X - cx, rp.Y, rp.Z - cz, rp.Temperature, rp.Luminosity);
                }

                var chunkPos = Vector3.Transform(new Vector3(cx, 0f, cz), galaxyRotation);
                var chunkData = new GalaxySectorChunkAggregatedData(
                    chunkRadius, aggData.Inclination, aggData.SpinAngle, localPoints);

                var chunk = new GalaxySectorChunk(target, chunkPos, chunkRadius * 3f, chunkData);
                results.Add(chunk);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySector)target);
        }
    }
}
