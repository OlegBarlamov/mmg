namespace FrameworkSDK.Logging
{
    internal class NullLogger : IFrameworkLogger
    {
        public void Log(string message, string module, FrameworkLogLevel level)
        {
        }
    }
}