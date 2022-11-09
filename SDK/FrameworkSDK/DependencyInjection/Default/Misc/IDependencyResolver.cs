using System;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public interface IDependencyResolver
    {
        object[] ResolveDependencies([NotNull] IServiceLocator serviceLocator, [NotNull] Type[] dependencies,
            [ItemNotNull] object[] parameters = null);
    }
}