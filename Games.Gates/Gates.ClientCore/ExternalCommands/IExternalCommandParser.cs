using JetBrains.Annotations;

namespace Gates.ClientCore.ExternalCommands
{
    internal interface IExternalCommandParser
    {
        [NotNull] ExternalCommand Parse(string commandLine);
    }
}
