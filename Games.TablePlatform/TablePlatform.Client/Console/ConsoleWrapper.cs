using Console.InGame;
using Console.LoggingAdapter;

namespace TablePlatform.Client.Console
{
    internal class ConsoleWrapper
    {
        public InGameConsoleController ConsoleController { get; set; }
        
        public LoggerConsoleMessagesProvider ConsoleMessagesProvider { get; set; }
        public ConsoleCommandExecutor ConsoleCommandExecutor { get; set; }
    }
}