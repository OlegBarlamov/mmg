using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Objects;

namespace X4World.Generation
{
    public class StarAsPointDetailsGenerator : IDetailsGenerator<StarAsPoint>
    {
        public IRandomService RandomService { get; }

        public StarAsPointDetailsGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        public void Generate(StarAsPoint target)
        {
            var aggregatedData = target.AggregatedData;
            var objects = new List<StarAsSphere>();
            
            for (int i = 0; i < aggregatedData.Power * 10; i++)
            {
                var position = RandomService.NextVector3(new Vector3(-50), new Vector3(50));
                var itemAggregatedData = new StarAsSphereAggregatedData(1);
                var item = new StarAsSphere(target, position, itemAggregatedData);
                objects.Add(item);
            };
            
            target.SetGeneratedData(objects);
        }
        
        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarAsPoint) target);
        }
    }
}