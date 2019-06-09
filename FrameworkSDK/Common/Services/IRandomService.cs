using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.Common
{
	public interface IRandomService
	{
		int NextInteger(int minValue, int maxValue);

		double NextDouble();

		Guid NewGuid();
	}
}