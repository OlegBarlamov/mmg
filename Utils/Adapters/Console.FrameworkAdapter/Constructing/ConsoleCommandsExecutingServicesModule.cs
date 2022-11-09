using Console.Core;
using Console.Core.CommandExecution;
using Console.Core.Commands;
using FrameworkSDK.DependencyInjection;

namespace Console.FrameworkAdapter.Constructing
{
    public class ConsoleCommandsExecutingServicesModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterType<IExecutableCommandsCollection, ExecutableCommandsCollection>();
            serviceRegistrator.RegisterType<IConsoleCommandExecutor, ExecutableConsoleCommandsExecutor>();
            
            var consoleCommandsSearcher = new AttributeConsoleCommandsSearcher();
            var commands = consoleCommandsSearcher.SearchCommands();
            foreach (var command in commands)
            {
                serviceRegistrator.RegisterType(typeof(IExecutableConsoleCommand), command);
            }
        }
    }
}