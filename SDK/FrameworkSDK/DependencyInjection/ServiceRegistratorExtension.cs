using System;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection
{
    public static class ServiceRegistratorExtension
    {
        public static void RegisterInstance<T>([NotNull] this IServiceRegistrator serviceRegistrator, [NotNull] T instance)
        {
            if (serviceRegistrator == null) throw new ArgumentNullException(nameof(serviceRegistrator));
            serviceRegistrator.RegisterInstance(typeof(T), instance);
        }

        public static void RegisterType<TService, TImpl>([NotNull] this IServiceRegistrator serviceRegistrator,
            ResolveType resolveType = ResolveType.Singletone) where TImpl : class, TService
        {
            if (serviceRegistrator == null) throw new ArgumentNullException(nameof(serviceRegistrator));
            serviceRegistrator.RegisterType(typeof(TService), typeof(TImpl), resolveType);
        }

	    public static void RegisterModule([NotNull] this IServiceRegistrator serviceRegistrator, [NotNull] IServicesModule module)
	    {
		    if (serviceRegistrator == null) throw new ArgumentNullException(nameof(serviceRegistrator));
		    if (module == null) throw new ArgumentNullException(nameof(module));
			module.RegisterServices(serviceRegistrator);
	    }

        public static void RegisterModule<TModule>([NotNull] this IServiceRegistrator serviceRegistrator)
            where TModule : class, IServicesModule
        {
            var module = Activator.CreateInstance<TModule>();
            serviceRegistrator.RegisterModule(module);
        }

        public static void RegisterFactory<TService>([NotNull] this IServiceRegistrator serviceRegistrator,
            [NotNull] ServiceFactoryDelegate factory, ResolveType resolveType = ResolveType.Singletone)
        {
            serviceRegistrator.RegisterFactory(typeof(TService), factory, resolveType);
        }
    }
}
