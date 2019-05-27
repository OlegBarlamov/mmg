using System;
using ConsoleWindow;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace FrameworkSDK.TraceSystem
{
	internal class TraceSubSystem : IDisposable
	{
		private TraceSettings TraceSettings { get; }

		[CanBeNull] private IConsoleHost _console;

		private readonly LogSystem _logSystem;
		private readonly ILogger _appLogger;

		public TraceSubSystem([NotNull] TraceSettings traceSettings)
		{
			TraceSettings = traceSettings ?? throw new ArgumentNullException(nameof(traceSettings));

			_logSystem = new LogSystem(TraceSettings.LogDirectoryPath, TraceSettings.IsDebug);
			_appLogger = _logSystem.CreateLogger("app");
		}

		public ILoggerFactory GetLoggerFactory()
		{
			return _logSystem;
		}

		public void Initialize()
		{
			if (TraceSettings.IsConsoleEnabled)
			{
				InitializeConsole();
			}
		}

		public void OnActivated(GameBase game)
		{
			if (_console != null)
			{
				game.IsMouseVisible = false;
				_console.KeyPressed += ConsoleOnKeyPressed;
				_console.TopMost = true;
			}
		}

		public void OnDeactivated(GameBase game)
		{
			if (_console != null)
			{
				game.IsMouseVisible = true;
				_console.KeyPressed -= ConsoleOnKeyPressed;
				_console.TopMost = false;
			}
		}

		public void Dispose()
		{
			_logSystem.OpenErrorsLogIfNeed();
			if (_console != null)
			{
				_console.KeyPressed -= ConsoleOnKeyPressed;
				_console.Dispose();
			}
			_logSystem.Dispose();
		}

		private void InitializeConsole()
		{
			_console = ConsoleFactory.CreateHosted();
			if (_console == null)
				return;

			_logSystem.AddProvider(_console);

			if (TraceSettings.IsConsoleShowed)
			{
				_console.TopMost = false;
				_console.Show();
			}
		}

		private void ConsoleOnKeyPressed(object sender, int e)
		{
			if (_console == null)
				return;

			if (e == 192)
			{
				_appLogger.Info("~ pressed");

				if (_console.IsShowed)
				{
					_console.Hide();
				}
				else
				{
					_console.Show();
				}
			}
		}
	}
}