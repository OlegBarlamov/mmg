using System.Windows.Threading;
using ConsoleWindow.Hosted;

namespace ConsoleWindow
{
    public static class ConsoleFactory
    {
		public static IConsole Create()
        {
            return new ConsoleController(Dispatcher.CurrentDispatcher, new ConsolePalette());
        }

        public static IConsole Create(Dispatcher dispatcher)
        {
			return new ConsoleController(dispatcher, new ConsolePalette());
        }

        public static IConsole Create(IConsolePalette consolePalette)
        {
			return new ConsoleController(Dispatcher.CurrentDispatcher, consolePalette);
        }

        public static IConsole Create(Dispatcher dispatcher, IConsolePalette consolePalette)
        {
			return new ConsoleController(dispatcher, consolePalette);
        }

        public static IConsoleHost CreateHosted()
        {
	        return new ConsoleHost();
        }
    }
}
