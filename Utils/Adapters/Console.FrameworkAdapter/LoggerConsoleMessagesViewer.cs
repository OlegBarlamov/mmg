using System;
using Console.Core;
using Console.Core.Models;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    public class LoggerConsoleMessagesViewer : IConsoleController
    {
        private IFrameworkLogger Logger { get; }

        public LoggerConsoleMessagesViewer([NotNull] IFrameworkLogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void Dispose()
        {
        }

        public bool IsShowed { get; } = false;
        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void ClearCurrent()
        {
        }

        public void ClearAll()
        {
        }

        public void AddMessage(IConsoleMessage consoleMessage)
        {
            Logger.Log(consoleMessage.Message, consoleMessage.Source, ConsoleLogLevelToFrameworkLogLevel(consoleMessage.LogLevel));
        }

        private static FrameworkLogLevel ConsoleLogLevelToFrameworkLogLevel(ConsoleLogLevel consoleLogLevel)
        {
            switch (consoleLogLevel)
            {
                case ConsoleLogLevel.Trace:
                    return FrameworkLogLevel.Trace;
                case ConsoleLogLevel.Debug:
                    return FrameworkLogLevel.Debug;
                case ConsoleLogLevel.Information:
                    return FrameworkLogLevel.Info;
                case ConsoleLogLevel.Warning:
                    return FrameworkLogLevel.Warn;
                case ConsoleLogLevel.Error:
                    return FrameworkLogLevel.Error;
                case ConsoleLogLevel.Critical:
                    return FrameworkLogLevel.Error;
                default:
                    throw new ArgumentOutOfRangeException(nameof(consoleLogLevel), consoleLogLevel, null);
            }
        }
    }
}