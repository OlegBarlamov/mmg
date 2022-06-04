using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.Core.Implementations.Terminal
{
    public class TerminalConsoleViewer : IConsoleController
    {
        public bool IsShowed => true;

        public ConsoleLogLevelHelper LogLevelHelper { get; } = new ConsoleLogLevelHelper();
        
        private IConsoleMessagesProvider ConsoleMessagesProvider { get; }

        private bool _started;
        private bool _isDisposed;
        private ConsoleLogLevel[] _allowedLevels = Array.Empty<ConsoleLogLevel>();
        
        private readonly CancellationTokenSource _lifeTimeTokenSource = new CancellationTokenSource();
        private readonly ConcurrentQueue<IConsoleMessage> _messagesForWrite = new ConcurrentQueue<IConsoleMessage>();
        private readonly bool _isAllowColored;
        
        public TerminalConsoleViewer([NotNull] IConsoleMessagesProvider consoleMessagesProvider)
        {
            ConsoleMessagesProvider = consoleMessagesProvider ?? throw new ArgumentNullException(nameof(consoleMessagesProvider));
            ConsoleMessagesProvider.NewMessages += ConsoleMessagesProviderOnNewMessages;

            _isAllowColored = IsAllowColored();
        }

        public void Initialize()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
            if (_started) throw new Exception($"TerminalConsoleViewer already initialized");
            
            _started = true;

            SetFilter(LogLevelHelper.DefaultFilter());

            Task.Factory.StartNew(ListenMessagesFunc, _lifeTimeTokenSource.Token, TaskCreationOptions.LongRunning,
                TaskScheduler.Current);
        }

        public void SetFilter([NotNull] ConsoleLogLevel[] allowedLevels)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
            
            _allowedLevels = allowedLevels ?? throw new ArgumentNullException(nameof(allowedLevels));
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
            
            _isDisposed = true;
            ConsoleMessagesProvider.NewMessages -= ConsoleMessagesProviderOnNewMessages;
            using (_lifeTimeTokenSource)
            {
                _lifeTimeTokenSource.Cancel();
            }
            
            while (!_messagesForWrite.IsEmpty)
            {
                _messagesForWrite.TryDequeue(out _);
            }
        }
        
        public void Show()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
        }

        public void Hide()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
        }

        public void ClearCurrent()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
            System.Console.Clear();
        }

        public void ClearAll()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TerminalConsoleViewer));
            System.Console.Clear();
        }

        public void AddMessage(IConsoleMessage consoleMessage)
        {
            _messagesForWrite.Enqueue(consoleMessage);
        }

        private async void ListenMessagesFunc()
        {
            var token = _lifeTimeTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    
                    while (!_messagesForWrite.IsEmpty)
                    {
                        if (_messagesForWrite.TryDequeue(out var message))
                            WriteMessageToConsole(message);
                        
                        token.ThrowIfCancellationRequested();
                    }

                    await Task.Delay(200, token).ConfigureAwait(true);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void ConsoleMessagesProviderOnNewMessages()
        {
            while (!ConsoleMessagesProvider.IsQueueEmpty)
            {
                var message = ConsoleMessagesProvider.Pop();
                _messagesForWrite.Enqueue(message);
            }
        }

        private void WriteMessageToConsole(IConsoleMessage message)
        {
            var level = message.LogLevel;
            if (!_allowedLevels.Contains(level))
                return;
            
            if (_isAllowColored)
                WriteMessageColored(message);
            else
                WriteMessageDefault(message);
        }

        private void WriteMessageColored(IConsoleMessage message)
        {
            //TODO
            WriteMessageDefault(message);
        }

        private void WriteMessageDefault(IConsoleMessage message)
        {
            var logLevelText = "";
            switch (message.LogLevel)
            {
                case ConsoleLogLevel.Trace:
                    break;
                case ConsoleLogLevel.Debug:
                    break;
                case ConsoleLogLevel.Information:
                    break;
                case ConsoleLogLevel.Warning:
                    logLevelText = "Warning: ";
                    break;
                case ConsoleLogLevel.Error:
                    logLevelText = "Error: ";
                    break;
                case ConsoleLogLevel.Critical:
                    logLevelText = "Fatal: ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var contentText = message.Content?.ToString() ?? string.Empty;
            var text = $"[{message.Source}]:{logLevelText}{message.Message}";

            var oldColor = System.Console.ForegroundColor;
            var color = GetColorByLogLevel(message.LogLevel);
            
            if (color != null)
                System.Console.ForegroundColor = color.Value;
            
            System.Console.WriteLine(text);
            if (!string.IsNullOrWhiteSpace(contentText))
                System.Console.WriteLine(contentText);
            
            if (color != null)
                System.Console.ForegroundColor = oldColor;
        }

        private static ConsoleColor? GetColorByLogLevel(ConsoleLogLevel logLevel)
        {
            if (logLevel == ConsoleLogLevel.Critical || logLevel == ConsoleLogLevel.Error)
                return ConsoleColor.Red;
            if (logLevel == ConsoleLogLevel.Warning)
                return ConsoleColor.Yellow;
            if (logLevel == ConsoleLogLevel.Debug)
                return ConsoleColor.Gray;

            return null;
        }

        private static bool IsAllowColored()
        {
            var platform = Environment.OSVersion.Platform;
            return platform == PlatformID.Unix || platform == PlatformID.MacOSX;
        }
        
        public class ConsoleLogLevelHelper
        {
            internal ConsoleLogLevelHelper()
            {
                
            }

            public ConsoleLogLevel[] All()
            {
                return new[]
                {
                    ConsoleLogLevel.Trace,
                    ConsoleLogLevel.Debug,
                    ConsoleLogLevel.Information,
                    ConsoleLogLevel.Warning,
                    ConsoleLogLevel.Error,
                    ConsoleLogLevel.Critical
                };
            }

            public ConsoleLogLevel[] None()
            {
                return Array.Empty<ConsoleLogLevel>();
            }

            public ConsoleLogLevel[] Errors()
            {
                return new[]
                {
                    ConsoleLogLevel.Error,
                    ConsoleLogLevel.Critical
                };
            }
            
            public ConsoleLogLevel[] ErrorsAndWarnings()
            {
                return new[]
                {
                    ConsoleLogLevel.Warning,
                    ConsoleLogLevel.Error,
                    ConsoleLogLevel.Critical
                };
            }
            
            public ConsoleLogLevel[] ErrorsWarningsAndInformation()
            {
                return new[]
                {
                    ConsoleLogLevel.Information,
                    ConsoleLogLevel.Warning,
                    ConsoleLogLevel.Error,
                    ConsoleLogLevel.Critical
                };
            }
            
            internal ConsoleLogLevel[] DefaultFilter()
            {
#if DEBUG
                return All();
#else
                return new[]
                {
                    ConsoleLogLevel.Information,
                    ConsoleLogLevel.Warning,
                    ConsoleLogLevel.Error,
                    ConsoleLogLevel.Critical
                };
#endif

            }
        }
    }
}