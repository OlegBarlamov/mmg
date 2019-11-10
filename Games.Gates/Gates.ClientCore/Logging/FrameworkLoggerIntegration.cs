using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace Gates.ClientCore.Logging
{
    internal class FrameworkLoggerIntegration : IFrameworkLogger
    {
	    private ILoggerFactory LoggerFactory { get; }

	    public FrameworkLoggerIntegration([NotNull] ILoggerFactory loggerFactory)
	    {
		    LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
	    }

	    public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
	    {
		    var logger = LoggerFactory.CreateLogger(module.ToString());
			logger.Log(message, ToDefaultLogLevel(level));
	    }

	    private static LogLevel ToDefaultLogLevel(FrameworkLogLevel frameworkLogLevel)
	    {
		    switch (frameworkLogLevel)
		    {
			    case FrameworkLogLevel.Trace: return LogLevel.Trace;
			    case FrameworkLogLevel.Debug: return LogLevel.Debug;
			    case FrameworkLogLevel.Info: return LogLevel.Information;
			    case FrameworkLogLevel.Warn: return LogLevel.Warning;
			    case FrameworkLogLevel.Error: return LogLevel.Error;
			    case FrameworkLogLevel.Fatal: return LogLevel.Critical;
			    default:
				    throw new ArgumentOutOfRangeException(nameof(frameworkLogLevel), frameworkLogLevel, null);
		    }
	    }
    }
}
