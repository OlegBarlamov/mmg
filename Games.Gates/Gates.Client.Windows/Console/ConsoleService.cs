using System;
using System.IO;
using ConsoleWindow;
using Gates.ClientCore;
using Gates.ClientCore.ExternalCommands;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Gates.Client.Windows.Console
{
    [UsedImplicitly]
	internal class ConsoleService : IExternalCommandsProvider
	{
		public event Action<string> NewCommand;

		private ILoggerFactory LoggerFactory { get; }

		private readonly IConsoleHost _console;

	    private readonly ToConsoleWriter _consoleStandartOutWriter;
	    private readonly ToConsoleWriter _consoleStandartErrorWriter;
	    private readonly TextWriter _defaultConsoleOut;
	    private readonly TextWriter _defaultConsoleError;

        public ConsoleService([NotNull] ILoggerFactory loggerFactory)
		{
			LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

			_console = ConsoleFactory.CreateHosted();
			LoggerFactory.AddProvider(_console);

            _console.UserCommand += ConsoleOnUserCommand;

		    _defaultConsoleOut = System.Console.Out;
		    _defaultConsoleError = System.Console.Error;
		    _consoleStandartOutWriter = new ToConsoleWriter(_console, LogLevel.Trace, "z_stdout");
		    _consoleStandartErrorWriter = new ToConsoleWriter(_console, LogLevel.Error, "z_!stderror");
		    System.Console.SetOut(_consoleStandartOutWriter);
		    System.Console.SetError(_consoleStandartErrorWriter);
        }

	    public void Dispose()
		{
			NewCommand = null;
		    _console.UserCommand -= ConsoleOnUserCommand;
		    System.Console.SetOut(_defaultConsoleOut);
		    System.Console.SetError(_defaultConsoleError);
		    _consoleStandartOutWriter.Dispose();
		    _consoleStandartErrorWriter.Dispose();
            _console.Dispose();
		}

		public void Open()
		{
			_console.Show();
		}

		public void Close()
		{
			_console.Hide();
		}

	    private void ConsoleOnUserCommand(object sender, string command)
	    {
	        NewCommand?.Invoke(command);
        }
    }
}
