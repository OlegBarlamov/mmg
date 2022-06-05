using System;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using X4World.Objects;

namespace X4World.Generation
{
    public class DefaultStarsMapGenerator : IStarsMapGenerator
    {
        public IRandomService RandomService { get; }

        public DefaultStarsMapGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        public void GenerateStarsForGalaxy(Galaxy galaxy)
        {
            var maxPos = galaxy.WorldPosition + galaxy.Size;
            var minPos = galaxy.WorldPosition - galaxy.Size;

            var stars = Enumerable.Range(0, 50).Select(i => new Star(RandomService.NextVector3(minPos, maxPos), galaxy));
            foreach (var star in stars)
            {
                galaxy.AddStar(star);
            }
        }
    }
}