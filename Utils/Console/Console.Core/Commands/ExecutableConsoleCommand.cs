using System;
using System.Threading.Tasks;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.Core.Commands
{
    public abstract class ExecutableConsoleCommand : IExecutableConsoleCommand
    {
        public abstract string Text { get; }

        public abstract string Description { get; }

        public abstract string Title { get; }

        public virtual object Data { get; protected set; } = null;
        
        IConsoleCommandMetadata IConsoleCommand.Metadata => _consoleCommandMetadata;

        private readonly ConsoleCommandMetadata _consoleCommandMetadata;
        
        public ExecutableConsoleCommand()
        {
            _consoleCommandMetadata = new ConsoleCommandMetadata(this);
        }
        
        public abstract Task ExecuteAsync(string command);

        private class ConsoleCommandMetadata : IConsoleCommandMetadata
        {
            public string Title => ExecutableConsoleCommand.Title;
            public string Description => ExecutableConsoleCommand.Description;
            public object Data => ExecutableConsoleCommand.Data;
            
            private ExecutableConsoleCommand ExecutableConsoleCommand { get; }

            public ConsoleCommandMetadata([NotNull] ExecutableConsoleCommand executableConsoleCommand)
            {
                ExecutableConsoleCommand = executableConsoleCommand ?? throw new ArgumentNullException(nameof(executableConsoleCommand));
            }
        }
    }
}