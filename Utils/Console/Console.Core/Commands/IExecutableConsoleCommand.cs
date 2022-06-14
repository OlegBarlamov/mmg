using System.Threading.Tasks;
using Console.Core.Models;

namespace Console.Core.Commands
{
    public interface IExecutableConsoleCommand : IConsoleCommand
    {
        Task ExecuteAsync(string command);
    }
}