using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceRegistrator
    {
        void RegisterInstance<T>([NotNull] T instance);

        void RegisterType<TService, TImpl>();
    }
}
