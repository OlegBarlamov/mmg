using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.Constructing
{
    public abstract class AppFactoryWrapper : IAppFactory
    {
        protected IAppFactory AppFactory { get; }

        public AppFactoryWrapper([NotNull] IAppFactory appFactory)
        {
            AppFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }
        
        public virtual IApp Construct()
        {
            return AppFactory.Construct();
        }

        public IAppFactory AddServices(IServicesModule module)
        {
            return AppFactory.AddServices(module);
        }

        public IAppFactory AddServices<TModule>() where TModule : class, IServicesModule
        {
            return AppFactory.AddServices<TModule>();
        }

        public IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent
        {
            return AppFactory.AddComponent<TComponent>();
        }

        public IAppFactory AddComponent(IAppComponent appComponent)
        {
            return AppFactory.AddComponent(appComponent);
        }
    }
}