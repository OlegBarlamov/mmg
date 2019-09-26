using Epic.Core;
using Epic.Core.Logging;
using FrameworkSDK.IoC;
using Microsoft.Extensions.Logging;

namespace Epic.Game
{
    internal class CommonModule : IServicesModule
    {
        private ILogFactory LogFactory { get; }
        private ILoggerFactory LoggerFactory { get; }

        public CommonModule(ILogFactory logFactory, ILoggerFactory loggerFactory)
        {
            LogFactory = logFactory;
            LoggerFactory = loggerFactory;
        }

        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterInstance(LogFactory);
            serviceRegistrator.RegisterInstance(LoggerFactory);

            serviceRegistrator.RegisterModule(new CoreModule());
        }
    }
}
