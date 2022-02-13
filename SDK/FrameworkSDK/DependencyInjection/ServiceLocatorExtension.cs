using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection
{
    public static class ServiceLocatorExtension
    {
        [NotNull]
        public static T Resolve<T>([NotNull] this IServiceLocator serviceLocator, params object[] parameters)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return (T)serviceLocator.Resolve(typeof(T), parameters);
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<T> ResolveMultiple<T>([NotNull] this IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return serviceLocator.ResolveMultiple(typeof(T)).Cast<T>().ToArray();
        }

        public static bool IsServiceRegistered<T>([NotNull] this IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            return serviceLocator.IsServiceRegistered(typeof(T));
        }
    }
}
