using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace AspNetCore.FrameworkAdapter
{
    internal class FrameworkBasedServiceProvider : IServiceProvider, IDisposable
    {
        public IFrameworkServiceContainer ScopedContainer { get; }
        public IServiceLocator ServiceLocator { get; private set; }

        public FrameworkBasedServiceProvider([NotNull] IFrameworkServiceContainer scopedContainer)
        {
            ScopedContainer = scopedContainer ?? throw new ArgumentNullException(nameof(scopedContainer));
        }
        
        public IServiceProvider Build()
        {
            ServiceLocator = ScopedContainer.BuildContainer();
            return this;
        }
        
        public object GetService([NotNull] Type serviceType)
        {
            if (!ServiceLocator.IsServiceRegistered(serviceType))
                return null;
            
            return ServiceLocator.Resolve(serviceType);
        }

        public void Dispose()
        {
            //TODO
            //ScopedContainer.Dispose();
        }
    }
}