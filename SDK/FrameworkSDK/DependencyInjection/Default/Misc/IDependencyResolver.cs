using System.Reflection;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default.Misc
{
    public interface IDependencyResolver
    {
        object[] ResolveDependencies([NotNull] IServiceLocator serviceLocator, [NotNull] ConstructorInfo constructor,
            [ItemNotNull] object[] parameters = null);
    }
}