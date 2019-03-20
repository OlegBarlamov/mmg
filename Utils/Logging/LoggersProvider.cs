using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Logging
{
    internal class LoggersProvider : ILoggerProvider
    {
        public const string LogExtension = "log";
        public const string ErrorLoggerName = "!Errors";
        public const string WarningLoggerName = "!Warnings";
        public const string DebugLoggerName = "!Debug";

        private string LogsDirectoryFullPath { get; }
        private bool IsDebug { get; }

        private readonly ConcurrentQueue<Serilog.ILogger> _createdDisposableLoggers = new ConcurrentQueue<Serilog.ILogger>();

        private Logger[] SystemLoggers { get; }

        public LoggersProvider(string logsDirectoryFullPath, bool isDebug)
        {
            LogsDirectoryFullPath = logsDirectoryFullPath ?? throw new ArgumentNullException(nameof(logsDirectoryFullPath));
            IsDebug = isDebug;

            SystemLoggers = new[]
            {
                SetupLevelLogger(ErrorLoggerName, LogEventLevel.Error).CreateLogger(),
                SetupLevelLogger(WarningLoggerName, LogEventLevel.Warning).CreateLogger(),
                SetupLevelLogger(DebugLoggerName, LogEventLevel.Debug).CreateLogger()
            };

        }

        public void Dispose()
        {
            while (_createdDisposableLoggers.TryDequeue(out var logger))
            {
                if (logger is IDisposable disposableLogger)
                    disposableLogger.Dispose();
            }

            foreach (var systemLogger in SystemLoggers)
                systemLogger.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            var configuration = SetupNewLogger(categoryName);
            return CreateLogger(configuration, categoryName);
        }

        private ILogger CreateLogger(LoggerConfiguration loggerConfiguration, string categoryName)
        {
            var internalLogger = loggerConfiguration.CreateLogger();

            _createdDisposableLoggers.Enqueue(internalLogger);

            return Convert(internalLogger, categoryName);
        }

        private LoggerConfiguration SetupLevelLogger(string categoryName, LogEventLevel level)
        {
            var config = CreateBaseConfig()
                .Filter.ByIncludingOnly(@event => @event.Level == level);

            return AddWriteToFileOption(config, categoryName);
        }

        private LoggerConfiguration SetupNewLogger(string categoryName)
        {
            var config = CreateBaseConfig()
                .WriteToLoggers(SystemLoggers);

            return AddWriteToFileOption(config, categoryName);
        }

        private LoggerConfiguration CreateBaseConfig()
        {
            return new LoggerConfiguration().SetupBase(IsDebug);
        }

        private LoggerConfiguration AddWriteToFileOption(LoggerConfiguration loggerConfiguration, string categoryName)
        {
            var outputFileName = CategoryNameToLogFileName(categoryName);
            var outputFilePath = Path.Combine(LogsDirectoryFullPath, outputFileName);
            return loggerConfiguration.WriteToFile(outputFilePath);
        }

        private ILogger Convert(Serilog.ILogger serilogLogger, string categoryName)
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            // ReSharper disable once ArgumentsStyleLiteral
            using (var serilogProvider = new SerilogLoggerProvider(serilogLogger, dispose: false))
            {
                return serilogProvider.CreateLogger(categoryName);
            }
        }

        private string CategoryNameToLogFileName(string categoryName)
        {
            return $"{categoryName}-" + "{Date}" + $".{LogExtension}";
        }
    }
}
