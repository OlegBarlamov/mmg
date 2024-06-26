﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Logging
{
    public class LogSystem : ILoggerFactory
    {
        private LogSystemConfig Config { get; }
        private readonly ILoggerFactory _internalLoggerFactory = new LoggerFactory();

        public LogSystem([NotNull] LogSystemConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            var internalProvider = new LoggersProvider(config);

            _internalLoggerFactory.AddProvider(internalProvider);
        }

        public void Dispose()
        {
            _internalLoggerFactory.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _internalLoggerFactory.CreateLogger(categoryName);
        }

        public void OpenErrorsLogIfNeed()
        {
            if (!Config.IsDebug)
                return;

            if (GetLoggerOutputFilePath(LoggersProvider.ErrorLoggerName, out var errorLogOutputFilePath))
                OpenFile(errorLogOutputFilePath);
            else if (GetLoggerOutputFilePath(LoggersProvider.WarningLoggerName, out var warningLogOutputFilePath))
                OpenFile(warningLogOutputFilePath);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _internalLoggerFactory.AddProvider(provider);
        }

        private bool GetLoggerOutputFilePath(string loggerName, out string outputFilePath)
        {
            var files = Directory.GetFiles(Config.LogDirectory.FullName, $"{loggerName}*.{LoggersProvider.LogExtension}");
            outputFilePath = files.FirstOrDefault();
            return !string.IsNullOrWhiteSpace(outputFilePath);
        }

        private void OpenFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    Process.Start(filePath);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
