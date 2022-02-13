using Console.Core;
using Console.Core.Implementations;
using Console.InGame;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    [UsedImplicitly]
    internal class InGameConsoleServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IConsoleController, InGameConsoleController>();
            serviceRegistrator.RegisterType<IConsoleMessagesProvider, EmptyConsoleMessagesProvider>();
            serviceRegistrator.RegisterType<IConsoleCommandExecutor, EmptyConsoleCommandExecutor>();
            serviceRegistrator.RegisterType<IConsoleResourcePackage, DefaultResourcePackage>();
            serviceRegistrator.RegisterType<DefaultConsoleManipulator, DefaultConsoleManipulator>();
        }
    }
}