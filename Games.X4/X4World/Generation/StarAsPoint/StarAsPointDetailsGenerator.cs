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
            var planetsCount = RandomService.NextInteger(1, 30);

            var planetsPositions = new List<Vector3>();
            for (int i = 0; i < planetsCount; i++)
            {
                var position = RandomService.NextVector3(new Vector3(-50), new Vector3(50));
                planetsPositions.Add(position);
            };
            
            var itemAggregatedData = new PlanetSystemFarthestAggregatedData(planetsPositions, target.AggregatedData);
            var item = new PlanetSystemFarthest(target, Vector3.Zero, itemAggregatedData);

            target.SetGeneratedData(new []{item});
        }
        
        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarAsPoint) target);
        }
    }
}