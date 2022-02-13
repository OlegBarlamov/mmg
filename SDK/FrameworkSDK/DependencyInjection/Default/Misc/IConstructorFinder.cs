using System;
using System.Reflection;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public interface IConstructorFinder
    {
        ConstructorInfo GetConstructor([NotNull] IServiceLocator serviceLocator, [NotNull] Type type, [ItemNotNull] Type[] parametersTypes = null);
    }
}