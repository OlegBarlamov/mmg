﻿using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    public class AutoDefferedLogger : IFrameworkLogger, IDisposable
    {
        private Func<IFrameworkLogger> LoggerProviderFunc { get; set; }

        private bool _isDisposed;
        private readonly ManualDeferredLogger _deferredLogger;

        public AutoDefferedLogger([NotNull] Func<IFrameworkLogger> loggerProviderFunc)
        {
            LoggerProviderFunc = loggerProviderFunc ?? throw new ArgumentNullException(nameof(loggerProviderFunc));

            _deferredLogger = new ManualDeferredLogger();
        }

        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AutoDefferedLogger));

            var targetLogger = LoggerProviderFunc.Invoke();
            if (targetLogger == null)
            {
                _deferredLogger.Log(message, module, level);
                return;
            }

            if (!_deferredLogger.IsEmpty)
                _deferredLogger.LogTo(targetLogger);

            targetLogger.Log(message, module, level);
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AutoDefferedLogger));

            _isDisposed = true;
            _deferredLogger.Dispose();
            LoggerProviderFunc = null;
        }
    }
}
