using System;
using System.Reflection;
using FrameworkSDK.DependencyInjection.Default.Misc;

namespace FrameworkSDK.DependencyInjection.Default
{
    internal static class ConstructorFinderExtensions
    {
        public static ConstructorInfo GetConstructorWithParameters(this IConstructorFinder finder, IServiceLocator serviceLocator, Type targetType,
            params Type[] parameters)
        {
            return finder.GetConstructor(serviceLocator, targetType, parameters);
        }
    }
}
