using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.Services
{
    [UsedImplicitly]
    public class FixedDetailsGeneratorProvider : IDetailsGeneratorProvider
    {
        private readonly IReadOnlyDictionary<Type, IDetailsGenerator> _dictionary;

        public FixedDetailsGeneratorProvider(IRandomService randomService)
        {
            _dictionary = new Dictionary<Type, IDetailsGenerator>
            {
                {typeof(WorldMapCellContent), new WorldMapCellContentDetailsGenerator()},
                {typeof(GalaxiesBatch), new GalaxiesBatchDetailsGenerator()},
                {typeof(GalaxyAsPoint), new GalaxyAsPointDetailsGenerator(randomService)},
                {typeof(GalaxyTextureFarthest), new GalaxyTextureFarthestDetailsGenerator()},
                {typeof(GalaxyTextureLayered), new GalaxyTextureLayeredDetailsGenerator()},
                {typeof(GalaxySectorTexture), new GalaxySectorTextureDetailsGenerator()},
                {typeof(GalaxySector), new GalaxySectorDetailsGenerator()},
                {typeof(GalaxySectorChunk), new GalaxySectorChunkDetailsGenerator()},
                {typeof(StarSystemsBatch), new StarSystemsBatchDetailsGenerator()},
                {typeof(StarSystemAsPoint), new StarSystemAsPointDetailsGenerator()},
                {typeof(StarSystemAsLightPoint), new StarSystemAsLightPointDetailsGenerator()}
            };
        }
        
        public IDetailsGenerator GetGenerator(IGeneratorTarget target)
        {
            var targetType = target.GetType();
            return _dictionary[targetType];
        }
    }
}