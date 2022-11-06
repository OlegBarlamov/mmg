using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;

namespace FrameworkSDK.Constructing
{
    public abstract class FrameworkContextAppFactory : IAppFactory
    {
        public abstract IApp Construct();
        public abstract IAppFactory AddServices(IServicesModule module);
        public abstract IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent;
        public abstract IAppFactory AddComponent(IAppComponent appComponent);

        protected void InitializeAppContext(IFrameworkLogger logger, IServiceLocator serviceLocator)
        {
            AppContext.Initialize(logger, serviceLocator);
        }
    }
}