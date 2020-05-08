using FrameworkSDK.IoC;

namespace Autofac.FrameworkAdapter
{
    public class AutofacServiceContainerFactory : IServiceContainerFactory
    {
        private readonly ContainerBuilder _containerBuilder = new ContainerBuilder();

        public IFrameworkServiceContainer CreateContainer(string name = null)
        {
            return new AutofacServiceContainer(_containerBuilder, name);
        }
    }
}