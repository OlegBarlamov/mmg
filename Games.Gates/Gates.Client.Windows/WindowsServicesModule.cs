using FrameworkSDK.IoC;
using Gates.Client.Windows.Console;
using Gates.ClientCore;
using Gates.ClientCore.ExternalCommands;

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