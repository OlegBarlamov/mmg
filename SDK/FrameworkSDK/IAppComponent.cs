using System;

namespace FrameworkSDK
{
    public interface IAppComponent : IDisposable
    {
        void Configure();
    }
}