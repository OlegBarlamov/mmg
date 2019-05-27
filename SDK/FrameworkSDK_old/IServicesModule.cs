using Autofac;

namespace FrameworkSDK
{
	public interface IServicesModule
	{
		void RegisterServices(ContainerBuilder container);
	}

	public class EmptyModule : IServicesModule
	{
		public void RegisterServices(ContainerBuilder container)
		{
		}
	}
}
