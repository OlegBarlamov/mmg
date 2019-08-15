using System;
using System.Reflection;

namespace FrameworkSDK.IoC.Default
{
    internal static class ConstructorFinderExtensions
    {
        public static ConstructorInfo GetConstructorWithParameters(this ConstructorFinder finder, Type targetType,
            params Type[] parameters)
        {
            return finder.GetConstructorWithParameters(targetType, parameters);
        }
    }
}
