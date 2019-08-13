using FrameworkSDK.Configuration;

namespace FrameworkSDK.Constructing
{
    public interface IAppConfigureHandler
    {
        PhaseConfiguration Configuration { get; }

        /// <summary>
        /// Осуществляет построение приложения и запуск.
        /// </summary>
        void Run();
    }
}