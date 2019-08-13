using System;
using FrameworkSDK.Configuration;

namespace FrameworkSDK.Constructing
{
    public interface IAppConfigureHandler : IDisposable
    {
        PhaseConfiguration Configuration { get; }

        /// <summary>
        /// Осуществляет построение приложения и запуск.
        /// </summary>
        void Run();
    }
}