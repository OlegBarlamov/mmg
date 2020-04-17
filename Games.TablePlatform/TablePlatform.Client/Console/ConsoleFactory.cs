using Console.LoggingAdapter;

namespace TablePlatform.Client.Console
{
    internal static class ConsoleFactory
    {
        public static ConsoleWrapper CreateConsole()
        {
            var logMessagesProvider = new LoggerConsoleMessagesProvider();
            var commandExecutor = new ConsoleCommandExecutor();
            return new ConsoleWrapper
            {
                ConsoleCommandExecutor =  commandExecutor,
                ConsoleMessagesProvider = logMessagesProvider
            };
        }
    }
}