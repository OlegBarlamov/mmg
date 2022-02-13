using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    /// <summary>
    /// Logger interface used by Framework
    /// </summary>
    public interface IFrameworkLogger
    {
        void Log([NotNull] string message, FrameworkLogModule module, FrameworkLogLevel level);
    }
}
