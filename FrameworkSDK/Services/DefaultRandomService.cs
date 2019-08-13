using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Common
{
	public class DefaultRandomService : IRandomService
	{
	    private readonly Random _seed;

	    public DefaultRandomService([NotNull] Random seed)
	    {
	        _seed = seed ?? throw new ArgumentNullException(nameof(seed));
	    }

		public int NextInteger(int minValue, int maxValue)
		{
			return _seed.Next(minValue, maxValue);
		}

		public double NextDouble()
		{
			return _seed.NextDouble();
		}

	    public Guid NewGuid()
	    {
	        return Guid.NewGuid();
	    }
	}
}
