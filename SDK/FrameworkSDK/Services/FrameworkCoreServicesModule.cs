using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.DependencyInjection.Default;
using FrameworkSDK.DependencyInjection.Default.Misc;
using FrameworkSDK.Services.Default;
using FrameworkSDK.Services.Randoms;
using JetBrains.Annotations;

namespace FrameworkSDK.Services
{
    [UsedImplicitly]
    internal class FrameworkCoreServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IAppSubSystemsRunner, DefaultSubSystemsRunner>();
            serviceRegistrator.RegisterType<IRandomSeedProvider, GuidBasedSeedProvider>();
            serviceRegistrator.RegisterType<IRandomService, DefaultRandomService>();
            serviceRegistrator.RegisterType<IAppDomainService, AppDomainService>();
            serviceRegistrator.RegisterType<IServiceLocator, AppContextServiceLocator>();
            serviceRegistrator.RegisterType<IConstructorFinder, DefaultConstructorFinder>();
            serviceRegistrator.RegisterType<IDependencyResolver, DefaultDependencyResolver>();
            serviceRegistrator.RegisterType<IServicesCandidatesFinder, DefaultServicesCandidatesFinder>();
            serviceRegistrator.RegisterType<IServicesRegistrationsPriority, DefaultServicesRegistrationsPriority>();
            serviceRegistrator.RegisterType<DefaultServiceLocatorFactory, DefaultServiceLocatorFactory>();
        }
    }
}