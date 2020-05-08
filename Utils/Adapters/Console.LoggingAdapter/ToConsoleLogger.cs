using System;
using Console.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Console.LoggingAdapter
{
    internal class ToConsoleLogger : ILogger
    {
        private class EmptyScope : IDisposable
        {
            void IDisposable.Dispose()
            {
            }
        }

        private static IDisposable EmptyScopeInstance { get; } = new EmptyScope();
        
        private IToConsoleWriter ToConsoleWriter { get; }
        private string CategoryName { get; }

        public ToConsoleLogger([NotNull] IToConsoleWriter toConsoleWriter, [NotNull] string categoryName)
        {
            ToConsoleWriter = toConsoleWriter ?? throw new ArgumentNullException(nameof(toConsoleWriter));
            CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var consoleLogLevel = ToConsoleLogLevel(logLevel);
            var text = formatter?.Invoke(state, exception) ?? string.Empty;
            var messageInstance = new ConsoleMessage
            {
                Message = text,
                LogLevel = consoleLogLevel,
                Source =  CategoryName,
                Content = state
            };
            
            SendMessage(messageInstance);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return EmptyScopeInstance;
        }

        private void SendMessage(IConsoleMessage message)
        {
            ToConsoleWriter.WriteMessageToConsole(message);
        }

        private static ConsoleLogLevel ToConsoleLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return ConsoleLogLevel.Trace;
                case LogLevel.Debug:
                    return ConsoleLogLevel.Debug;
                case LogLevel.Information:
                    return ConsoleLogLevel.Information;
                case LogLevel.Warning:
                    return ConsoleLogLevel.Warning;
                case LogLevel.Error:
                    return ConsoleLogLevel.Error;
                case LogLevel.Critical:
                    return ConsoleLogLevel.Critical;
                case LogLevel.None:
                    return ConsoleLogLevel.Information;
                default:
                    return ConsoleLogLevel.Information;
            }
        }
    }
}