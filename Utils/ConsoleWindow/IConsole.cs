using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow
{
    public interface IConsole : ILoggerProvider, ILogger
    {
        bool IsShowed { get; }

        bool TopMost { get; set; }

        void Show();

        void Hide();

        void ClearCurrent();

        void ClearAll();

        void Write(string message, LogLevel logLevel = LogLevel.Information, string sourceName = null);

        event EventHandler<string> UserCommand;

        Collection<CommandDescription> KnownCommands { get; }

        string ShowedSource { get; }
    }
}
