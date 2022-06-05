using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Console.FrameworkAdapter.Commands.Types;

namespace Console.FrameworkAdapter.Commands
{
    public abstract class TypedExecutableConsoleCommand : FixedParametersExecutableCommand
    {
        public override string Title => _title ?? (_title = GenerateTitle());
        protected override int MinimumParametersNumber => ExpectedTypes.Length;
        
        protected abstract IConsoleCommandTypeDescription[] ExpectedTypes { get; }
        
        private string _title;
        
        protected sealed override Task ExecuteAsyncWithVerifiedParametersNumber(string[] parameters)
        {
            var parsedValues = new List<object>();
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                var expectedType = ExpectedTypes[index];
                if (!expectedType.IsParsable(parameter))
                    throw new ConsoleCommandExecutionException($"Wrong type for console command parameter {parameter}. Expected type: {expectedType.Title}");
                
                parsedValues.Add(expectedType.Parse(parameter));
            }

            return ExecuteAsyncWithVerifiedParametersTypes(parsedValues);
        }

        protected abstract Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters);

        private string GenerateTitle()
        {
            var convertTypeToString = ExpectedTypes.Select((type, i) => i + 1 > MinimumParametersNumber ? $"[{type.Title}]" : type.Title); 
            return $"{Text} {string.Join(" ", convertTypeToString)}";
        }
    }
}