namespace FrameworkSDK.IoC
{
    public interface IServiceLocator
    {
        T Resolve<T>();
    }
}
