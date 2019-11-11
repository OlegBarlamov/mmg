using System.Linq;

namespace Gates.ClientCore.ExternalCommands
{
    internal class ExternalCommandParser : IExternalCommandParser
    {
        public ExternalCommand Parse(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return ExternalCommand.Empty();

            var words = commandLine.Trim()
                .Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            if (words.Length == 0)
                return ExternalCommand.Empty();

            var commandName = words.First();
            var parameters = words.Skip(1).ToArray();

            return new ExternalCommand(commandName, parameters);
        }
    }
}
