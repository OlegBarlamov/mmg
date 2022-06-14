using System;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Commands;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.InGame.Commands
{
    public class AddLogLevelFilterCommand : FixedTypedExecutableConsoleCommand<ConsoleLogLevel>
    {
        public override string Text { get; } = "console.filterlevel";
        public override string Description { get; } = "Filter console messages by log level";
        
        private IConsoleController ConsoleController { get; }
        private InGameConsoleController InGameConsoleController { get; }

        public AddLogLevelFilterCommand([NotNull] IConsoleController consoleController)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            InGameConsoleController = ConsoleController as InGameConsoleController;
        }
        
        protected override Task ExecuteAsync(ConsoleLogLevel parameter)
        {
            if (InGameConsoleController == null)
                throw new ConsoleCommandExecutionException("Passed consoleController is not default InGameConsoleController. Console messages can not be filtered.");
            
            InGameConsoleController.RenderingMessagesFilter.Add(parameter);
            return Task.CompletedTask;
        }
    }
    
    public class AddSourceFilterCommand : FixedTypedExecutableConsoleCommand<string>
    {
        public override string Text { get; } = "console.filtersource";
        public override string Description { get; } = "Filter console messages by source";
        
        private IConsoleController ConsoleController { get; }
        private InGameConsoleController InGameConsoleController { get; }

        public AddSourceFilterCommand([NotNull] IConsoleController consoleController)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            InGameConsoleController = ConsoleController as InGameConsoleController;
        }
        
        protected override Task ExecuteAsync(string source)
        {
            if (InGameConsoleController == null)
                throw new ConsoleCommandExecutionException("Passed consoleController is not default InGameConsoleController. Console messages can not be filtered.");
            
            InGameConsoleController.RenderingMessagesFilter.Add(source);
            return Task.CompletedTask;
        }
    }
    
    
    public class ClearFilterCommand : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; } = "console.filterreset";
        public override string Description { get; } = "Reset filtering for console messages";
        
        private IConsoleController ConsoleController { get; }
        private InGameConsoleController InGameConsoleController { get; }

        public ClearFilterCommand([NotNull] IConsoleController consoleController)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            InGameConsoleController = ConsoleController as InGameConsoleController;
        }
        
        protected override Task ExecuteAsync()
        {
            if (InGameConsoleController == null)
                throw new ConsoleCommandExecutionException("Passed consoleController is not default InGameConsoleController. Console messages can not be filtered.");
            
            InGameConsoleController.RenderingMessagesFilter.ClearFilters();
            return Task.CompletedTask;
        }
    }
}