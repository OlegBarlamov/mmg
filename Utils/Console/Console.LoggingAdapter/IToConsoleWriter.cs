using Console.Core.Models;

namespace Console.LoggingAdapter
{
    internal interface IToConsoleWriter
    {
        void WriteMessageToConsole(IConsoleMessage message);
    }
}