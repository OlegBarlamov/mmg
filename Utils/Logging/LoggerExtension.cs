using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Logging
{
    public static class LoggerExtension
    {
        public static void Log(this ILogger logger, string message, LogLevel logLevel = LogLevel.Information)
        {
            logger.LogWithAdditionalInfo(logLevel, message);
        }

        public static void Info(this ILogger logger, string message)
        {
            logger.LogWithAdditionalInfo(LogLevel.Information, message);
        }

        public static void Info(this ILogger logger, Exception exception, string message)
        {
            if (exception == null)
                logger.Info(message);
            else
                logger.LogException(exception, LogLevel.Information, message);
        }

        public static void Error(this ILogger logger, string message)
        {
            logger.LogWithAdditionalInfo(LogLevel.Error, message);
        }

        public static void Error(this ILogger logger, Exception exception, string message)
        {
            if (exception == null)
                logger.Error(message);
            else
                logger.LogException(exception, LogLevel.Error, message);
        }

        public static void Critical(this ILogger logger, string message)
        {
            logger.LogWithAdditionalInfo(LogLevel.Critical, message);
        }

        public static void Critical(this ILogger logger, Exception exception, string message)
        {
            if (exception == null)
                logger.Critical(message);
            else
                logger.LogException(exception, LogLevel.Critical, message);
        }

        public static void Warning(this ILogger logger, string message)
        {
            logger.LogWithAdditionalInfo(LogLevel.Warning, message);
        }

        public static void Warning(this ILogger logger, Exception exception, string message)
        {
            if (exception == null)
                logger.Warning(message);
            else
                logger.LogException(exception, LogLevel.Warning, message);
        }

        public static void Debug(this ILogger logger, string message)
        {
            logger.LogWithAdditionalInfo(LogLevel.Debug, message);
        }

        public static void Debug(this ILogger logger, Exception exception, string message)
        {
            if (exception == null)
                logger.Debug(message);
            else
                logger.LogException(exception, LogLevel.Debug, message);
        }

        private static void LogException(this ILogger logger, Exception exception, LogLevel logLevel, string message)
        {
            logger.LogWithAdditionalInfo(logLevel, message + Environment.NewLine + exception);
        }

        private static void LogWithAdditionalInfo(this ILogger logger, LogLevel logLevel, string message)
        {
            var time = DateTime.Now;
            var ln = Environment.NewLine;
            var outputMessage = $"[{time:yyyy-MM-dd hh:mm:ss,fffff} {ThreadName()}]{ln}" +
                                LogLevelToStr(logLevel, ln) +
                                message;

            logger.Log(logLevel, EventId, string.Empty, null,(s, exception) =>  outputMessage);
        }

		private static readonly EventId EventId = new EventId();

        private static string ThreadName()
        {
            var currentThread = Thread.CurrentThread;
            var result = string.IsNullOrWhiteSpace(currentThread.Name)
                ? currentThread.ManagedThreadId.ToString()
                : currentThread.Name;

            return $"Thread={result}";
        }

        private static string LogLevelToStr(LogLevel logLevel, string newLineStr)
        {
            var ln = newLineStr;
            var logLevelStr = logLevel.ToString().ToUpperInvariant();

            if (logLevel == LogLevel.Error)
                return $"{logLevelStr}!{ln}";
            if (logLevel == LogLevel.Critical)
                return $"{logLevelStr}!!!{ln}";

            return string.Empty;
        }
    }
}
