using System;
using FrameworkSDK.DependencyInjection;

namespace FrameworkSDK.Common
{
    internal class LazyService<TService> where TService : class
    {
        public TService Service => _lazy.Value;

        private readonly Lazy<TService> _lazy =  new Lazy<TService>(ServiceFactory);

        private static TService ServiceFactory()
        {
            return AppContext.ServiceLocator.Resolve<TService>();
        }
    }
}
