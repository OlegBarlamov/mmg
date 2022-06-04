using System.Threading.Tasks;

namespace Console.FrameworkAdapter.Commands
{
    public abstract class FixedParametersExecutableCommand : ParametrizedExecutableConsoleCommand
    {
        protected abstract int MinimumParametersNumber { get; }
        
        protected sealed override Task ExecuteAsyncWithParameters(string[] parameters)
        {
            if (parameters.Length < MinimumParametersNumber)
                throw new ConsoleCommandExecutionException($"Wrong console command {Text} parameters number. Expected: {MinimumParametersNumber}, Actual: {parameters.Length}");

            return ExecuteAsyncWithVerifiedParametersNumber(parameters);
        }

        protected abstract Task ExecuteAsyncWithVerifiedParametersNumber(string[] parameters);
    }
}