using System;

namespace FrameworkSDK.Common
{
	public class DefaultRandomService : IRandomService
	{
		private readonly Random _seed = new Random(Guid.NewGuid().GetHashCode());

		public int NextInteger(int minValue, int maxValue)
		{
			return _seed.Next(minValue, maxValue);
		}

		public double NextDouble()
		{
			return _seed.NextDouble();
		}
	}
}
