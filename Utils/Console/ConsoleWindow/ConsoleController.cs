using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using ConsoleWindow.Models;
using ConsoleWindow.ViewModels;
using ConsoleWindow.Views;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow
{
    internal class ConsoleController : IConsole
    {
	    public event EventHandler<string> UserCommand;

        Collection<CommandDescription> IConsole.KnownCommands => _commandDescriptions;
        public string ShowedSource => _viewModel.SelectedConsoleContent.Name;

        internal readonly Views.ConsoleWindow Window = new Views.ConsoleWindow();

		private Dispatcher Dispatcher { get; }
        private IConsolePalette ConsolePalette { get; }

        private bool _disposed;

        private readonly ObservableCollection<CommandDescription> _commandDescriptions = new ObservableCollection<CommandDescription>();
        private readonly ConsoleControlViewModel _viewModel;
        private readonly ConsoleSourceCollection _sourceCollection;
        private readonly NullLogger _nullLogger = new NullLogger();

        public ConsoleController([NotNull] Dispatcher dispatcher, [NotNull] IConsolePalette consolePalette)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            ConsolePalette = consolePalette ?? throw new ArgumentNullException(nameof(consolePalette));
            
            _sourceCollection = new ConsoleSourceCollection();

            _viewModel = new ConsoleControlViewModel(_sourceCollection, Dispatcher, ConsolePalette,
                new ReadOnlyObservableCollection<CommandDescription>(_commandDescriptions));
            _viewModel.UserCommand += ConsoleViewModelOnUserCommand;
            var control = new ConsoleControlView
            {
                DataContext = _viewModel
            };

            Window.Content = control;
            Window.Closing += WindowOnClosing;
        }

        public void Dispose()
        {
            _disposed = true;

            Window.Closing -= WindowOnClosing;
            Window.Close();

            _viewModel.UserCommand -= ConsoleViewModelOnUserCommand;
            _viewModel.Dispose();
            _sourceCollection.Dispose();
            _nullLogger.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return _nullLogger;

            return _sourceCollection.CreateSource(categoryName);
        }

        public bool IsShowed => Window.IsVisible;

        public bool TopMost
        {
            get => Window.Topmost;
            set => Window.Topmost = value;
        }

        public void Show()
        {
            if (_disposed)
                return;

            Window.Show();
            Window.Activate();
        }

        public void Hide()
        {
            if (_disposed)
                return;

            Window.Hide();
        }

        public void ClearCurrent()
        {
            _viewModel.ClearSelected();
        }

        public void ClearAll()
        {
            _viewModel.ClearAll();
        }

        public void Write(string message, LogLevel logLevel = LogLevel.Information, string sourceName = null)
        {
            _sourceCollection.WriteMessage(message, logLevel, sourceName);
        }

	    public void Write(string message, ConsoleColor color, string sourceName = null)
	    {
		    _sourceCollection.WriteMessage(message, LogLevel.Information, sourceName, color);
		}

		private void ConsoleViewModelOnUserCommand(object sender, string commandText)
        {
            UserCommand?.Invoke(this, commandText);
        }

        private void WindowOnClosing(object sender, CancelEventArgs e)
        {
            if (_disposed)
                return;

            e.Cancel = true;
            Hide();
        }

        private class NullLogger : ILogger, IDisposable
        {
            void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
            }

            bool ILogger.IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            IDisposable ILogger.BeginScope<TState>(TState state)
            {
                return this;
            }

            public void Dispose()
            {
            }
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception);
            Write(message, logLevel);
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return !_disposed;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return new FakeDisposable();
        }
    }

    internal class FakeDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}