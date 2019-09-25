namespace FrameworkSDK.MonoGame.Mvc
{
	internal static class MvcSchemeExtensions
	{
		public static bool IsValid(this IMvcScheme scheme)
		{
			return scheme.View != null || scheme.Model != null || scheme.Controller != null;
		}
	}
}
