using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace X4World.Generation
{
    public class GalaxyAsPointDetailsGenerator : IDetailsGenerator<GalaxyAsPoint>
    {
        public IRandomService RandomService { get; }

        public GalaxyAsPointDetailsGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        public void Generate(GalaxyAsPoint target)
        {
            var aggregatedData = target.AggregatedData;
            var objects = new List<SphereAsPoint>();
            
            for (int i = 0; i < aggregatedData.Power * 10; i++)
            {
                var position = RandomService.NextVector3(new Vector3(-50), new Vector3(50));
                var itemAggregatedData = new SphereAsPointAggregatedData(1);
                var item = new SphereAsPoint(target, position, itemAggregatedData);
                objects.Add(item);
            };
            
            target.SetGeneratedData(objects);
        }
        
        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyAsPoint) target);
        }
    }
}