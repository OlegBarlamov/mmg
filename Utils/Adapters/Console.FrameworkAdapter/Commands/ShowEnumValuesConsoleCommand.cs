using System;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Commands;
using FrameworkSDK.Services;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter.Commands
{
    [RegisterConsoleCommand]
    public class ShowEnumValuesConsoleCommand : FixedTypedExecutableConsoleCommand<string>
    {
        public IAppDomainService AppDomainService { get; }
        public IConsoleController ConsoleController { get; }
        public override string Text { get; } = "net.enum";
        public override string Description { get; } = "Get enum values from .net enum type name";

        public ShowEnumValuesConsoleCommand([NotNull] IAppDomainService appDomainService, [NotNull] IConsoleController consoleController)
        {
            AppDomainService = appDomainService ?? throw new ArgumentNullException(nameof(appDomainService));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
        }
        
        protected override Task ExecuteAsync(string parameter)
        {
            var enumType = AppDomainService.FindTypeFromShortName(parameter);
            if (enumType == null)
                throw new ConsoleCommandExecutionException("Invalid type name");
            if (!enumType.IsEnum)
                throw new ConsoleCommandExecutionException($"Target type {enumType.Name} is not Enum type");
            
            ConsoleController.AddMessage($"[{string.Join(",", Enum.GetNames(enumType))}]");
            return Task.CompletedTask;
        }
    }
}