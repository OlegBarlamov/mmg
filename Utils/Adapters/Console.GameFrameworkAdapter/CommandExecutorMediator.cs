using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    public class CommandExecutorMediator : IConsoleCommandExecutor, IDisposable
    {
        public Action<string> Command;

        private bool _disposed;
        
        private readonly List<IConsoleCommand> _consoleCommands;
        private readonly object _commandsLocker = new object();

        public CommandExecutorMediator(params IConsoleCommand[] commands)
            : this((IEnumerable<IConsoleCommand>)commands)
        {
        }
        
        public CommandExecutorMediator(IEnumerable<IConsoleCommand> commands)
        {
            _consoleCommands = new List<IConsoleCommand>(commands);
        }
        
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CommandExecutorMediator));
            _disposed = true;
            
            lock (_commandsLocker)
            {
                _consoleCommands.Clear();   
            }
            
            Command = null;
        }
        
        public IEnumerable<IConsoleCommand> GetAvailableCommands()
        {
            lock (_commandsLocker)
            {
                return _consoleCommands.ToArray();
            }
        }

        public void AddCommand([NotNull] IConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (_disposed) throw new ObjectDisposedException(nameof(CommandExecutorMediator));

            lock (_commandsLocker)
            {
                _consoleCommands.Add(command);
            }
        }

        public void RemoveCommand([NotNull] IConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (_disposed) throw new ObjectDisposedException(nameof(CommandExecutorMediator));
            
            lock (_commandsLocker)
            {
                _consoleCommands.Remove(command);
            }
        }

        public Task ExecuteAsync(string command)
        {
            Command?.Invoke(command);
            
            return Task.CompletedTask;
        }
    }
}