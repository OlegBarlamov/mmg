using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    internal class LoggerShell : IFrameworkLogger
    {
        public bool IsUsedCustomLogger => CustomLogger != null;

        [CanBeNull] private IFrameworkLogger CustomLogger { get; set; }

        [NotNull] private IFrameworkLogger ActiveLogger => CustomLogger ?? _defaultLogger;

        [NotNull] private readonly IFrameworkLogger _defaultLogger = new NullLogger();

        public void SetupLogger([CanBeNull] IFrameworkLogger logger)
        {
			CustomLogger = logger;
        }

        void IFrameworkLogger.Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
            ActiveLogger.Log(message, module, level);
        }
    }
}