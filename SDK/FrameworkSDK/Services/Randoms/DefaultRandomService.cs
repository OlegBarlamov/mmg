using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;

namespace FrameworkSDK.Services.Randoms
{
	public class DefaultRandomService : IRandomService
	{
	    private Random Seed => RandomSeedProvider.Seed;

	    [NotNull] private IRandomSeedProvider RandomSeedProvider { get; }

	    public DefaultRandomService([NotNull] IRandomSeedProvider randomSeedProvider)
	    {
	        RandomSeedProvider = randomSeedProvider ?? throw new ArgumentNullException(nameof(randomSeedProvider));
	    }

		public int NextInteger(int minValue, int maxValue)
		{
			return Seed.Next(minValue, maxValue);
		}

		public double NextDouble()
		{
			return Seed.NextDouble();
		}

	    public Guid NewGuid()
	    {
	        return Guid.NewGuid();
	    }
	}
}
