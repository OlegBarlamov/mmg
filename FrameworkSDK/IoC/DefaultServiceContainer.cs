using System;
using System.ComponentModel.Design;

namespace FrameworkSDK.IoC
{
    internal class DefaultServiceContainer : IFrameworkServiceContainer, IServiceLocator
    {
        public DefaultServiceContainer()
        {
            var c = new ServiceContainer();
            
        }

        public void RegisterInstance<T>(T instance)
        {
            
        }

        public void RegisterType<TService, TImpl>()
        {
            throw new NotImplementedException();
        }

        public IServiceLocator BuildContainer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }
    }
}
