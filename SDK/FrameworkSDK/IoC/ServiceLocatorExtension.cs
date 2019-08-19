using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public static class ServiceLocatorExtension
    {
        [NotNull]
        public static T Resolve<T>([NotNull] this IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return (T)serviceLocator.Resolve(typeof(T));
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<T> ResolveMultiple<T>([NotNull] this IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return (IReadOnlyList<T>)serviceLocator.ResolveMultiple(typeof(T));
        }

        public static bool IsServiceRegistered<T>([NotNull] this IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return serviceLocator.IsServiceRegistered(typeof(T));
        }

        [NotNull]
        public static T ResolveWithParamter<T>([NotNull] this IServiceLocator serviceLocator, params object[] paramters)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));

            return (T)serviceLocator.ResolveWithParameters(typeof(T), paramters);
        }
    }
}
