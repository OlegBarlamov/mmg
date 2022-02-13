using System;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection
{
    public class ServicesModuleDelegate : IServicesModule
    {
        private Action<IServiceRegistrator> RegisterDelegate { get; }

        public ServicesModuleDelegate([NotNull] Action<IServiceRegistrator> registerDelegate)
        {
            RegisterDelegate = registerDelegate ?? throw new ArgumentNullException(nameof(registerDelegate));
        }
        
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            RegisterDelegate.Invoke(serviceRegistrator);
        }
    }
}