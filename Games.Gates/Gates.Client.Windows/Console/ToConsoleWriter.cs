using System;
using System.IO;
using System.Text;
using ConsoleWindow;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Gates.Client.Windows.Console
{
    internal class ToConsoleWriter : TextWriter
    {
        public override Encoding Encoding { get; } = Encoding.Default;

        private string SourceName { get; }
        private IConsole Console { get; }
        private LogLevel LogLevel { get; }

        private string _bufferedLine = string.Empty;

        public ToConsoleWriter([NotNull] IConsole console, LogLevel logLevel, [NotNull] string sourceName)
        {
            Console = console ?? throw new ArgumentNullException(nameof(console));
            LogLevel = logLevel;
            SourceName = sourceName ?? throw new ArgumentNullException(nameof(sourceName));
        }

        public override void Write(char value)
        {
            _bufferedLine += value;
            if (IsNeedToNewLine(_bufferedLine))
            {
                WriteToConsole(_bufferedLine);
                _bufferedLine = string.Empty;
            }
        }

        private static bool IsNeedToNewLine(string str)
        {
            return str.EndsWith(Environment.NewLine);
        }

        private void WriteToConsole(string s)
        {
            Console.Write(s, LogLevel, SourceName);
        }
    }
}
