using JetBrains.Annotations;

namespace Console.Core.Models
{
    public interface IConsoleMessage
    {
        [NotNull] string Message { get; }
        
        [NotNull] string Source { get; }
        
        ConsoleLogLevel LogLevel { get; }
        
        [CanBeNull] object Content { get; }
    }
}