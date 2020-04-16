using System;
using System.Windows.Threading;
using ConsoleWindow.Common;
using ConsoleWindow.Models;
using ConsoleWindow.Views;
using JetBrains.Annotations;
using Brush = System.Windows.Media.Brush;

namespace ConsoleWindow.ViewModels
{
    internal class ConsoleContentViewModel : NotifyObject, IDisposable
    {
        [UsedImplicitly]
        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        public ConsoleDocument Content { get; }

        private ConsoleSource ConsoleSource { get; }
        private IConsolePalette ConsolePalette { get; }
        private Dispatcher Dispatcher { get; }

        private string _name;

        private readonly DispatcherDelayedExecutor _dispatcherDelayedExecutor;

        public ConsoleContentViewModel([NotNull] ConsoleSource consoleSource, [NotNull] IConsolePalette consolePalette, [NotNull] Dispatcher dispatcher)
        {
            ConsoleSource = consoleSource ?? throw new ArgumentNullException(nameof(consoleSource));
            ConsolePalette = consolePalette ?? throw new ArgumentNullException(nameof(consolePalette));
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            Name = consoleSource.Name;
            Content = new ConsoleDocument();

            _dispatcherDelayedExecutor = new DispatcherDelayedExecutor(Dispatcher, DispatcherPriority.Background, TimeSpan.FromMilliseconds(100));

            ConsoleSource.NewMessage += ConsoleSourceOnNewMessage;
        }

        public void Dispose()
        {
            ConsoleSource.NewMessage -= ConsoleSourceOnNewMessage;
            _dispatcherDelayedExecutor.Dispose();
        }

        public void Clear()
        {
            if (!Dispatcher.CheckAccess())
            {
                _dispatcherDelayedExecutor.EnqueueAction(ClearInternal);
            }
            else
            {
                ClearInternal();
            }
        }

        private void ConsoleSourceOnNewMessage(object sender, ConsoleMessage e)
        {
            if (string.IsNullOrEmpty(e.Message))
                return;

            //event for main thread. Must order safe!
            _dispatcherDelayedExecutor.EnqueueAction(() => AppendMessage(e));
        }

        private void AppendMessage(ConsoleMessage consoleMessage)
        {
            var text = consoleMessage.Message.Trim(' ', '\n', '\r');
            var brush = GetColor(consoleMessage);
            Content.AppendLine(text, brush);
        }

        private void ClearInternal()
        {
            Content.Blocks.Clear();
        }

	    private Brush GetColor(ConsoleMessage message)
	    {
		    var color = message.Color;
		    if (color != null)
		    {
			    var brush = ConsolePalette.FindBrush(color.Value);
			    if (brush != null)
				    return brush;
		    }
		    var logLevel = message.Level;
		    return ConsolePalette.FindBrush(logLevel) ?? ConsolePalette.DefaultBrush;
	    }
    }
}