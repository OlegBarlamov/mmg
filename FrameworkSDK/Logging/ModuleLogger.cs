using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    internal class ModuleLogger : IFrameworkLogger
    {
        [NotNull] private IFrameworkLogger FrameworkLogger { get; }

        private FrameworkLogModule LogModule { get; }

        public ModuleLogger([NotNull] IFrameworkLogger frameworkLogger, FrameworkLogModule logModule)
        {
            FrameworkLogger = frameworkLogger ?? throw new ArgumentNullException(nameof(frameworkLogger));

            LogModule = logModule;
        }

        public void Info(string message)
        {
            Log(message, FrameworkLogLevel.Info);
        }

        public void Trace(string message)
        {
            Log(message, FrameworkLogLevel.Trace);
        }

        public void Debug(string message)
        {
            Log(message, FrameworkLogLevel.Debug);
        }

        public void Warn(string message)
        {
            Log(message, FrameworkLogLevel.Warn);
        }

        public void Error(string message)
        {
            Log(message, FrameworkLogLevel.Error);
        }

        public void Fatal(string message)
        {
            Log(message, FrameworkLogLevel.Fatal);
        }

        public void Log(string message, FrameworkLogLevel level = FrameworkLogLevel.Info)
        {
            FrameworkLogger.Log(message, LogModule, level);
        }

        void IFrameworkLogger.Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            FrameworkLogger.Log(message, module, level);
        }
    }
}
