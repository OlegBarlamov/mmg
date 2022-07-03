using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using X4World.Generation;
using X4World.Objects;

namespace Atom.Client.MacOS.Services
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
                {typeof(GalaxyAsPoint), new GalaxyAsPointDetailsGenerator(randomService)}
            };
        }
        
        public IDetailsGenerator GetGenerator(IGeneratorTarget target)
        {
            var targetType = target.GetType();
            return _dictionary[targetType];
        }
    }
}