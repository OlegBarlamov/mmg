using JetBrains.Annotations;

namespace FrameworkSDK.Logging
{
    public interface IFrameworkLogger
    {
        void Log([NotNull] string message, FrameworkLogModule module, FrameworkLogLevel level);
    }
}
