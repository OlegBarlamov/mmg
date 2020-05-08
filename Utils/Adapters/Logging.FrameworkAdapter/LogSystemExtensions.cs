using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Logging.FrameworkAdapter
{
    public static class LogSystemExtensions
    {
        public static IFrameworkLogger ToFrameworkLogger([NotNull] this LogSystem logSystem)
        {
            if (logSystem == null) throw new ArgumentNullException(nameof(logSystem));
            return new FrameworkLoggerWrapper(logSystem);
        }
    }
}