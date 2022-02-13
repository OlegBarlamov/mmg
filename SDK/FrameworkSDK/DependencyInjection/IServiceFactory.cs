using System;

namespace FrameworkSDK.DependencyInjection
{
    public interface IServiceFactory
    {
        object CreateInstance(Type type, object[] parameters = null);
    }
}