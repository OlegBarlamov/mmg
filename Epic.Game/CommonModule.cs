using Epic.Core;
using Epic.Core.Logging;
using FrameworkSDK.IoC;

namespace Epic.Game
{
    internal class CommonModule : IServicesModule
    {
        private ILogFactory LogFactory { get; }

        public CommonModule(ILogFactory logFactory)
        {
            LogFactory = logFactory;
        }

        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterInstance(LogFactory);

            serviceRegistrator.RegisterModule(new CoreModule());
        }
    }
}
