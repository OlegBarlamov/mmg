namespace FrameworkSDK.IoC
{
    public interface IFrameworkServiceContainer : IServiceRegistrator
    {
        IServiceLocator BuildContainer();
    }
}
