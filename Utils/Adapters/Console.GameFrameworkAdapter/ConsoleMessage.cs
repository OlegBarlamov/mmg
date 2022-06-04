using Console.Core.Models;

namespace Console.FrameworkAdapter
{
    public class ConsoleMessage : IConsoleMessage
    {
        public string Message { get; }
        public string Source { get; }
        public ConsoleLogLevel LogLevel { get; }
        public object Content { get; }

        public ConsoleMessage(string message, string source, ConsoleLogLevel logLevel, object content = null)
        {
            Message = message;
            Source = source;
            LogLevel = logLevel;
            Content = content;
        }
    }
}