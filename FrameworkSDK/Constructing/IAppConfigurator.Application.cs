using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
	public static partial class AppConfiguratorExtensions
	{
		public static IAppConfigurator UseApplication<TApplication>([NotNull] this IAppConfigurator configurator) where TApplication : Application
		{
			
		}
	}
}
