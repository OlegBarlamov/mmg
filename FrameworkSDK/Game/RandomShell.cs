using FrameworkSDK.Common;

namespace FrameworkSDK.Game
{
	internal static class RandomShell
	{
		public static IRandomService RandomService { get; private set; }

		public static void Setup(IRandomService randomService)
		{
			RandomService = randomService;
		}
	}
}
