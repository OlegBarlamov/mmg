using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Logging;

namespace Atom.Client.Logging
{
	internal class FrameworkLoggerAdapter : IFrameworkLogger
	{
		private Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory { get; }

		public FrameworkLoggerAdapter([NotNull] Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
		{
			LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		public void Log(string message, FrameworkLogModule module, FrameworkLogLevel level)
		{
			LoggerFactory.CreateLogger(module.ToString()).Log(message, ConvertFrameworkLogLevel(level));
		}

		private static LogLevel ConvertFrameworkLogLevel(FrameworkLogLevel logLevel)
		{
			switch (logLevel)
			{
				case FrameworkLogLevel.Trace:
					return LogLevel.Trace;
				case FrameworkLogLevel.Debug:
					return LogLevel.Debug;
				case FrameworkLogLevel.Info:
					return LogLevel.Information;
				case FrameworkLogLevel.Warn:
					return LogLevel.Warning;
				case FrameworkLogLevel.Error:
					return LogLevel.Error;
				case FrameworkLogLevel.Fatal:
					return LogLevel.Critical;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}