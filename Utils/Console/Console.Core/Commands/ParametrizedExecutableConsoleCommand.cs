using System;
using System.Linq;
using System.Threading.Tasks;

namespace Console.Core.Commands
{
    public abstract class ParametrizedExecutableConsoleCommand : ExecutableConsoleCommand
    {
        public sealed override Task ExecuteAsync(string command)
        {
            var words = command.Split(' ');
            if (words.Length < 2)
                return ExecuteAsyncWithParameters(Array.Empty<string>());

            return ExecuteAsyncWithParameters(words.Skip(1).ToArray());
        }

        protected abstract Task ExecuteAsyncWithParameters(string[] parameters);
    }
}