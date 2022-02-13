using FrameworkSDK.DependencyInjection;

namespace FrameworkSDK
{
    public interface IAppFactory
    {
        IApp Construct();
        IAppFactory AddServices(IServicesModule module);
        IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent;
    }
}