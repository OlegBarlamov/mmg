using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    public class ExecutableCommandsCollection : IExecutableCommandsCollection
    {
        private readonly ConcurrentDictionary<string, IExecutableConsoleCommand> _registeredCommands;

        public ExecutableCommandsCollection([NotNull] IServiceLocator serviceLocator)
        {
            if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
            
            var commands = serviceLocator.ResolveMultiple<IExecutableConsoleCommand>();
            _registeredCommands = new ConcurrentDictionary<string, IExecutableConsoleCommand>(commands
                .Select(x => new KeyValuePair<string, IExecutableConsoleCommand>(x.Text, x)));
        }
        
        public IReadOnlyDictionary<string, IExecutableConsoleCommand> GetAvailableCommands()
        {
            return _registeredCommands;
        }
        
        public void AddCommand([NotNull] IExecutableConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _registeredCommands.AddOrUpdate(command.Text, command, (s, consoleCommand) => command);
        }

        public void RemoveCommand([NotNull] IExecutableConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _registeredCommands.TryRemove(command.Text, out _);
        }
    }
}