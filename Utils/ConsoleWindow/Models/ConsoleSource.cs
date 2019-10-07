using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow.Models
{
    internal class ConsoleSource : ILogger
    {
        public event EventHandler<ConsoleMessage> NewMessage;

        public string Name { get; }

        private readonly IReadOnlyList<ILogger> _subLoggers = Array.Empty<ILogger>();
        private readonly EventId _emptyEventId = new EventId();
        private readonly object _loggingLocker = new object();

        public ConsoleSource(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

            Name = name;
        }

        public ConsoleSource(string name, IReadOnlyList<ILogger> subLoggers)
            : this(name)
        {
            _subLoggers = subLoggers;
        }

        public void Write(string message, LogLevel logLevel, ConsoleColor? color = null)
        {
            Log<object>(logLevel, _emptyEventId, null, null, (o, exception) => message, color);
        }

        protected virtual bool Filter<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return true;
        }

        private void RaiseNewMessage(ConsoleMessage message)
        {
            NewMessage?.Invoke(this, message);
        }

        private void RaiseNewMessage(string message, LogLevel logLevel, ConsoleColor? color = null)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var consoleMessage = new ConsoleMessage(message, logLevel, color);
            RaiseNewMessage(consoleMessage);
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(logLevel, eventId, state, exception, formatter, null);
        }

        private void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, ConsoleColor? color)
        {
            lock (_loggingLocker)
            {
                if (!Filter(logLevel, eventId, state, exception, formatter))
                    return;

                LogSelf(logLevel, eventId, state, exception, formatter, color);
                LogToSubLoggers(logLevel, eventId, state, exception, formatter);
            }
        }

        private void LogSelf<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter, ConsoleColor? color = null)
        {
            if (formatter == null)
                return;

            var message = formatter(state, exception);
            RaiseNewMessage(message, logLevel, color);
        }

        private void LogToSubLoggers<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            foreach (var subLogger in _subLoggers)
            {
                subLogger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return new FakeDisposable();
        }
    }

    internal class FilteredByLevelConsoleSource : ConsoleSource
    {
        [NotNull] private readonly Func<LogLevel, bool> _filter;

        public FilteredByLevelConsoleSource(string name, [NotNull] Func<LogLevel, bool> filter) : base(name)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public FilteredByLevelConsoleSource(string name, Func<LogLevel, bool> filter, IReadOnlyList<ILogger> subLoggers) : base(name, subLoggers)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        protected override bool Filter<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            return _filter(logLevel);
        }
    }
}