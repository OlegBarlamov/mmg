using System.Collections.Generic;
using Console.Core.Models;
using Console.FrameworkAdapter.Commands;

namespace Console.FrameworkAdapter
{
    public interface IExecutableCommandsCollection
    {
        void AddCommand(IExecutableConsoleCommand command);
        void RemoveCommand(IExecutableConsoleCommand command);
        IReadOnlyDictionary<string, IExecutableConsoleCommand> GetAvailableCommands();
    }
}