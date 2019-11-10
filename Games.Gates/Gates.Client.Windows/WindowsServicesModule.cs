using FrameworkSDK.IoC;
using Gates.ClientCore;

namespace Gates.Client.Windows
{
    internal class WindowsServicesModule : IServicesModule
    {
        public void Register(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExternalCommandsProvider, ConsoleService>();
        }
    }
}