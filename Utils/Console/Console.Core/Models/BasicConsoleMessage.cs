namespace Console.Core.Models
{
    public class BasicConsoleMessage : IConsoleMessage
    {
        public string Message { get; }
        public string Source { get; }
        public ConsoleLogLevel LogLevel { get; }
        public object Content { get; }

        public BasicConsoleMessage(string message, string source, ConsoleLogLevel logLevel, object content = null)
        {
            Message = message;
            Source = source;
            LogLevel = logLevel;
            Content = content;
        }
    }
}