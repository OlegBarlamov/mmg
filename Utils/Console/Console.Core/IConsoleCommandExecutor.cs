using System.Collections.Generic;
using System.Threading.Tasks;
using Console.Core.Models;

namespace Console.Core
{
    public interface IConsoleCommandExecutor
    {
        IEnumerable<IConsoleCommand> GetAvailableCommands();

        Task ExecuteAsync(string command);
    }
}