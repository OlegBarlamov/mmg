using System;
using ConsoleWindow;
using Gates.ClientCore;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Gates.Client.Windows
{
	internal class ConsoleService : IExternalCommandsProvider
	{
		public event Action<string> NewCommand;

		private ILoggerFactory LoggerFactory { get; }

		private readonly IConsoleHost _console;

		public ConsoleService([NotNull] ILoggerFactory loggerFactory)
		{
			LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

			_console = ConsoleFactory.CreateHosted();
			LoggerFactory.AddProvider(_console);
		}

		public void Dispose()
		{
			NewCommand = null;
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
	}
}
