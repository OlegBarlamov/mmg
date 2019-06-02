using System;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceRegistrator : IDisposable
    {
        void RegisterInstance<T>([NotNull] T instance);

        void RegisterType<TService, TImpl>(ResolveType resolveType = ResolveType.Singletone);
    }
}
