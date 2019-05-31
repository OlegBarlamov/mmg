using System;

namespace FrameworkSDK.IoC
{
    public interface IServiceLocator : IDisposable
    {
        T Resolve<T>();
    }
}
