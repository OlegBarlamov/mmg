using System.Threading.Tasks;
using Console.Core.Models;

namespace Console.FrameworkAdapter.Commands
{
    public interface IExecutableConsoleCommand : IConsoleCommand
    {
        Task ExecuteAsync(string command);
    }
}