using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    public class ManualDeferredLogger : IFrameworkLogger, IDisposable
    {
        public bool IsEmpty => _defferedMessages.IsEmpty;

        private class LogMessage
        {
            public string Message { get; set; }
            public FrameworkLogModule Module { get; set; }
            public FrameworkLogLevel Level { get; set; }
        }

        private bool _isDisposed;
        private readonly ConcurrentQueue<LogMessage> _defferedMessages = new ConcurrentQueue<LogMessage>();

        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            if (_isDisposed)
                return;

            var logMessage = new LogMessage
            {
                Message = message,
                Module = module,
                Level = level
            };

            _defferedMessages.Enqueue(logMessage);
        }

        public void LogTo([NotNull] IFrameworkLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (_isDisposed) throw new ObjectDisposedException(nameof(ManualDeferredLogger));

            while (_defferedMessages.TryDequeue(out var m))
                logger.Log(m.Message, m.Module, m.Level);
        }

        public void Dispose()
        {
            _isDisposed = true;
            while (_defferedMessages.TryDequeue(out _))
            {

            }
        }
    }
}
