using System;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    internal static class ServiceRegistratorExtension
    {
        public static void RegisterInstance<T>([NotNull] this IServiceRegistrator serviceRegistrator, [NotNull] T instance)
        {
            if (serviceRegistrator == null) throw new ArgumentNullException(nameof(serviceRegistrator));
            serviceRegistrator.RegisterInstance(typeof(T), instance);
        }

        public static void RegisterType<TService, TImpl>([NotNull] this IServiceRegistrator serviceRegistrator,
            ResolveType resolveType = ResolveType.Singletone)
        {
            if (serviceRegistrator == null) throw new ArgumentNullException(nameof(serviceRegistrator));
            serviceRegistrator.RegisterType(typeof(TService), typeof(TImpl), resolveType);
        }
    }
}
