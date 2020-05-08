using Console.Core.Models;

namespace Console.LoggingAdapter
{
    public class ConsoleMessage : IConsoleMessage
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public ConsoleLogLevel LogLevel { get; set; }
        public object Content { get; set; }
    }
}