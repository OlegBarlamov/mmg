using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    public class InMemoryLogger : IFrameworkLogger, IDisposable
    {
        private readonly ConcurrentQueue<MemoryLogMessage> _memoryMessages = new ConcurrentQueue<MemoryLogMessage>();
        private bool _isDisposed;
        
        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            if (_isDisposed)
                return;
            
            var memoryMessage = new MemoryLogMessage
            {
                Message = message,
                Module = module,
                Level = level
            };
            
            _memoryMessages.Enqueue(memoryMessage);
        }

        public void LogTo([NotNull] IFrameworkLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (ReferenceEquals(logger, this))
                throw new ArgumentException(nameof(logger)); //TODO correct message

            while (!_memoryMessages.IsEmpty)
            {
                if (_memoryMessages.TryDequeue(out var message))
                {
                    logger.Log(message.Message, message.Module, message.Level);
                }
            }
        }

        private class MemoryLogMessage
        {
            public string Message { get; set; }
            public FrameworkLogModule Module { get; set; }
            public FrameworkLogLevel Level { get; set; }
        }

        public void Dispose()
        {
            _isDisposed = true;
            while (!_memoryMessages.IsEmpty)
            {
                _memoryMessages.TryDequeue(out _);
            }
        }
    }
}