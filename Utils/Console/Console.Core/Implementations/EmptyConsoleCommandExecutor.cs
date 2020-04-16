using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Console.Core.Models;

namespace Console.Core.Implementations
{
    public class EmptyConsoleCommandExecutor : IConsoleCommandExecutor
    {
        public IEnumerable<IConsoleCommand> GetAvailableCommands()
        {
            return Array.Empty<IConsoleCommand>();
        }

        public Task ExecuteAsync(string command)
        {
            return Task.CompletedTask;
        }
    }
}