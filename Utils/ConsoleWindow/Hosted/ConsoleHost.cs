using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow.Hosted
{
	internal class ConsoleHost : IConsoleHost
	{
		public event EventHandler<int> KeyPressed;

		private ConsoleController _console;

		private readonly ConsoleHostApp _hostApp;
		private readonly InterceptKeys _keys;

		public ConsoleHost()
		{
			_hostApp = CreateHostApp();

			InvokeToHost(InitializeConsole);

			_keys = new InterceptKeys();
			_keys.KeyPressed += KeysOnKeyPressed;
		}

		public void Dispose()
		{
			_keys.KeyPressed -= KeysOnKeyPressed;
			_keys.Dispose();

			SafeInvokeToHost(_console.Dispose);
			SafeInvokeToHost(_hostApp.Shutdown);
		}

		public void Invoke(Action action)
		{
			InvokeToHost(action);
		}

		public T Invoke<T>(Func<T> func)
		{
			T result = default(T);

			InvokeToHost(() => result = func());

			return result;
		}

		private void InitializeConsole()
		{
			_console = new ConsoleController(_hostApp.Dispatcher, new ConsolePalette());
		}

		private void KeysOnKeyPressed(object sender, Key e)
		{
			KeyPressed?.Invoke(this, (int)e);
		}

		private void InvokeToHost(Action action)
		{
			_hostApp.Dispatcher.Invoke(action);
		}

		private void SafeInvokeToHost(Action action)
		{
			try
			{
				_hostApp.Dispatcher.Invoke(action);
			}
			catch (Exception e)
			{
				// ignored
			}
		}

		private ConsoleHostApp CreateHostApp()
		{
			ConsoleHostApp host = null;
			var thread = new Thread(() =>
			{
				host = new ConsoleHostApp();
				host.Run();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();

			while (host == null)
				Thread.Sleep(50);

			return host;
		}

		ILogger ILoggerProvider.CreateLogger(string categoryName)
		{
			return _console.CreateLogger(categoryName);
		}

		void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
			((ILogger) _console).Log<TState>(logLevel, eventId, state, exception, formatter);
		}

		bool ILogger.IsEnabled(LogLevel logLevel)
		{
			return ((ILogger) _console).IsEnabled(logLevel);
		}

		IDisposable ILogger.BeginScope<TState>(TState state)
		{
			return ((ILogger) _console).BeginScope<TState>(state);
		}

		bool IConsole.IsShowed => _console.IsShowed;

		bool IConsole.TopMost
		{
			get => Invoke(() => _console.TopMost);
			set => Invoke(() => _console.TopMost = value);
		}

		void IConsole.Show()
		{
			Invoke(_console.Show);
		}

		void IConsole.Hide()
		{
			Invoke(_console.Hide);
		}

		void IConsole.ClearCurrent()
		{
			Invoke(_console.ClearCurrent);
		}

		void IConsole.ClearAll()
		{
			Invoke(_console.ClearAll);
		}

		void IConsole.Write(string message, LogLevel logLevel, string sourceName)
		{
			_console.Write(message, logLevel, sourceName);
		}

		event EventHandler<string> IConsole.UserCommand
		{
			add => _console.UserCommand += value;
			remove => _console.UserCommand -= value;
		}

		Collection<CommandDescription> IConsole.KnownCommands => ((IConsole) _console).KnownCommands;

		string IConsole.ShowedSource => _console.ShowedSource;
	}
}