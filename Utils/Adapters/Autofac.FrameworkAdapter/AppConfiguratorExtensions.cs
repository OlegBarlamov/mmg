using FrameworkSDK;
using JetBrains.Annotations;

namespace Autofac.FrameworkAdapter
{
    public static class AppFactoryExtensions
    {
        public static DefaultAppFactory UseAutofac([NotNull] this DefaultAppFactory appFactory)
        {
            var serviceContainer = new AutofacServiceContainer(new ContainerBuilder());
            return appFactory.UseServiceContainer(serviceContainer);
        }
    }
}