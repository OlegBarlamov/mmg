using System;

namespace FrameworkSDK
{
    public interface IGame : IDisposable
    {
        void Run();

        void Stop();
    }
}
