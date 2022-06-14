using Console.Core.Models;

namespace Console.Core
{
    public static class ConsoleControllerExtensions
    {
        public static void AddMessage(this IConsoleController consoleController, string message, string source = "console", ConsoleLogLevel consoleLogLevel = ConsoleLogLevel.Trace)
        {
            consoleController.AddMessage(new BasicConsoleMessage(
                message,
                source,
                consoleLogLevel
            ));
        }
    }
}