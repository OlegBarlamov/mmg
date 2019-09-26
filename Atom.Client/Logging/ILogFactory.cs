using Microsoft.Extensions.Logging;

namespace Atom.Client.Logging
{
    public interface ILogFactory
    {
        ILogger CreateLogger(string loggerName);
    }
}
