using System;
using FrameworkSDK.Common;

namespace FrameworkSDK.Services.Randoms
{
    internal class GuidBasedSeedProvider : IRandomSeedProvider
    {
        public Random Seed { get; } = new Random(Guid.NewGuid().GetHashCode());
    }

    internal class FixedSeedProvider : IRandomSeedProvider
    {
        public Random Seed { get; }
        private int SeedNumber { get; }

        public FixedSeedProvider()
            :this(0)
        {
            
        }

        public FixedSeedProvider(int seedNumber)
        {
            SeedNumber = seedNumber;
            Seed = new Random(SeedNumber);
        }
    }
}