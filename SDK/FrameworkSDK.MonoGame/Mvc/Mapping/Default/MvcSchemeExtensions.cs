namespace FrameworkSDK.MonoGame.Mvc
{
	internal static class MvcSchemeExtensions
	{
		public static bool IsValid(this IMvcComponentGroup componentGroup)
		{
			return componentGroup.View != null || componentGroup.Model != null || componentGroup.Controller != null;
		}
	}
}
