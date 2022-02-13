using System;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.Services
{
    [UsedImplicitly]
    internal class AppContextServiceLocator : IServiceLocator
    {
        public object Resolve(Type type, object[] parameters = null)
        {
            return AppContext.ServiceLocator.Resolve(type, parameters);
        }

        public Array ResolveMultiple(Type type)
        {
            return AppContext.ServiceLocator.ResolveMultiple(type);
        }

        public bool IsServiceRegistered(Type type)
        {
            return AppContext.ServiceLocator.IsServiceRegistered(type);
        }

        public object CreateInstance(Type type, object[] parameters)
        {
            return AppContext.ServiceLocator.CreateInstance(type, parameters);
        }
    }
}