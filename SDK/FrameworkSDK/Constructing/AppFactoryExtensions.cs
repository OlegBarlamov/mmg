using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    public static class AppFactoryExtensions
    {
        public static IAppFactory AddServices<TModule>(this IAppFactory appFactory) where TModule : class, IServicesModule
        {
            var module = Activator.CreateInstance<TModule>();
            return appFactory.AddServices(module);
        }

        public static IAppFactory AddServices(this IAppFactory appFactory, [NotNull] Action<IServiceRegistrator> registerServicesDelegate)
        {
            if (registerServicesDelegate == null) throw new ArgumentNullException(nameof(registerServicesDelegate));
            var module = new ServicesModuleDelegate(registerServicesDelegate);
            return appFactory.AddServices(module);
        }

        public static IAppFactory AddService<T>(this IAppFactory appFactory, [NotNull] T instance) where T: class
        {
            return appFactory.AddServices(registrator => { registrator.RegisterInstance(instance); });
        }
    }

}