namespace FrameworkSDK.Logging
{
    internal class NullLogger : IFrameworkLogger
    {
        public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
        {
        }
    }
}