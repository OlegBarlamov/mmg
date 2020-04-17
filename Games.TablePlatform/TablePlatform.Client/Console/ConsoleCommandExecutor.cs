using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Models;

namespace TablePlatform.Client.Console
{
    public class ConsoleCommandExecutor : IConsoleCommandExecutor
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