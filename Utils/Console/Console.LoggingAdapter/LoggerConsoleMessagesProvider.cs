using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Console.Core;
using Console.Core.Models;
using Microsoft.Extensions.Logging;

namespace Console.LoggingAdapter
{
    public class LoggerConsoleMessagesProvider : ILoggerProvider, IConsoleMessagesProvider, IToConsoleWriter
    {
        public event Action NewMessages;

        public bool IsQueueEmpty => _messagesToSend.Count == 0;

        private bool _isDisposed;
        
        private readonly Queue<IConsoleMessage> _messagesToSend = new Queue<IConsoleMessage>();
        private readonly ConcurrentDictionary<string, ToConsoleLogger> _loggers = new ConcurrentDictionary<string, ToConsoleLogger>();
        
        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(LoggerConsoleMessagesProvider));
            _isDisposed = true;

            while (_messagesToSend.Count > 0)
            {
                _messagesToSend.Dequeue();
            }

            _loggers.Clear();
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(LoggerConsoleMessagesProvider));
            
            return _loggers.GetOrAdd(categoryName, CreateLoggerInternal);
        }
        
        public IConsoleMessage Pop()
        {
            return _messagesToSend.Dequeue();
        }

        private ToConsoleLogger CreateLoggerInternal(string categoryName)
        {
            return new ToConsoleLogger(this, categoryName);
        }

        void IToConsoleWriter.WriteMessageToConsole(IConsoleMessage message)
        {
            if (!_isDisposed)
            {
                _messagesToSend.Enqueue(message);
                NewMessages?.Invoke();
            }
        }
    }
}