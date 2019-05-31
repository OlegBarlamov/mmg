namespace FrameworkSDK.IoC
{
    public interface IServiceRegistrator
    {
        void RegisterInstance<T>(T instance);

        void RegisterType<TService, TImpl>();
    }
}
