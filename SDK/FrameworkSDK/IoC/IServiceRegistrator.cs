using System;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceRegistrator
    {
        void RegisterInstance([NotNull] Type serviceType, [NotNull] object instance);

        void RegisterType([NotNull] Type serviceType, [NotNull] Type implType, ResolveType resolveType = ResolveType.Singletone);
    }
}
