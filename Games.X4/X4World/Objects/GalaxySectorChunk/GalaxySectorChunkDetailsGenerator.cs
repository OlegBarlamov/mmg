using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorChunkDetailsGenerator : IDetailsGenerator<GalaxySectorChunk>
    {
        public void Generate(GalaxySectorChunk target)
        {
            var aggData = target.AggregatedData;
            var batchData = new StarSystemsBatchAggregatedData(
                aggData.ChunkRadius, aggData.Inclination, aggData.SpinAngle, aggData.ClusterPoints);

            var batch = new StarSystemsBatch(target, Vector3.Zero,
                aggData.ChunkRadius * 2.0f, batchData);

            target.SetGeneratedData(new List<IWrappedDetails> { batch });
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorChunk)target);
        }
    }
}
