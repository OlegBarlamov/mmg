using System;
using System.IO;
using System.Text;
using System.Threading;
using ConsoleWindow;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Atom.Client.Services.Implementations
{
    [UsedImplicitly]
    internal class ConsoleService : IConsoleService, IDisposable
    {
        private ILoggerFactory LoggerFactory { get; }
        private readonly IConsoleHost _consoleHost;
        private readonly ToConsoleWriter _consoleStandartOutWriter;
        private readonly ToConsoleWriter _consoleStandartErrorWriter;
        private readonly TextWriter _defaultConsoleOut;
        private readonly TextWriter _defaultConsoleError;

	    private readonly AutoResetEvent _messageReceived = new AutoResetEvent(false);
	    private string _lastMessage;

		public ConsoleService([NotNull] ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _consoleHost = ConsoleFactory.CreateHosted();
            _consoleHost.TopMost = false;
			_consoleHost.UserCommand += ConsoleHostOnUserCommand;

            LoggerFactory.AddProvider(_consoleHost);

            _defaultConsoleOut = Console.Out;
            _defaultConsoleError = Console.Error;
            _consoleStandartOutWriter = new ToConsoleWriter(_consoleHost, LogLevel.Trace, "z_stdout");
            _consoleStandartErrorWriter = new ToConsoleWriter(_consoleHost, LogLevel.Error, "z_!stderror");
            Console.SetOut(_consoleStandartOutWriter);
            Console.SetError(_consoleStandartErrorWriter);
        }

	    public void Show()
        {
            _consoleHost.Show();
        }

        public void Hide()
        {
            _consoleHost.Hide();
        }

	    public void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
	    {
		    _consoleHost.Write(text, color);
	    }

	    public string ReadLine()
	    {
		    _messageReceived.WaitOne();
		    _messageReceived.Reset();
		    return _lastMessage;
	    }

	    public void Dispose()
        {
	        _consoleHost.UserCommand -= ConsoleHostOnUserCommand;
			Console.SetOut(_defaultConsoleOut);
            Console.SetError(_defaultConsoleError);
            _consoleStandartOutWriter.Dispose();
            _consoleStandartErrorWriter.Dispose();
            _consoleHost.Dispose();
		}

	    private void ConsoleHostOnUserCommand(object sender, string s)
	    {
		    _lastMessage = s;
		    _messageReceived.Set();
	    }

		private class ToConsoleWriter : TextWriter
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
}
