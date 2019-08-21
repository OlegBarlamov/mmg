using Microsoft.Extensions.Logging;

namespace Epic.Core.Logging
{
    public interface ILogFactory
    {
        ILogger CreateLogger(string loggerName);
    }
}
