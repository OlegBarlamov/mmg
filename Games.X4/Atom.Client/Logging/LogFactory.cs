using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace Atom.Client.Logging
{
    public class LogFactory : ILogFactory, Microsoft.Extensions.Logging.ILoggerFactory
    {
        private readonly LogSystem _logSystem;

        public LogFactory([NotNull] string logDirectory, bool isDebug)
        {
            if (logDirectory == null) throw new ArgumentNullException(nameof(logDirectory));
            _logSystem = new LogSystem(logDirectory, isDebug);
        }

        public IFrameworkLogger CreateAdapter()
        {
            return new FrameworkLoggerAdapter(_logSystem);
        }

        public void Dispose()
        {
            _logSystem.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logSystem.CreateLogger(categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _logSystem.AddProvider(provider);
        }
    }
}