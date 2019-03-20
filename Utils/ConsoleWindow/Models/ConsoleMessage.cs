using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ConsoleWindow.Models
{
    internal class ConsoleMessage
    {
        [NotNull] public string Message { get; }

        public LogLevel Level { get; }

        public ConsoleMessage([NotNull] string message, LogLevel level)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentException(nameof(message));

            Message = message;
            Level = level;
        }
    }
}
