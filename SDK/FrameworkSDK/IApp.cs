using System;

namespace FrameworkSDK
{
    public interface IApp : IDisposable
    {
        void Run();
    }
}