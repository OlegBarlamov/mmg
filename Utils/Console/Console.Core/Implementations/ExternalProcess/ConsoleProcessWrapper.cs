using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Console.Core.Implementations.ExternalProcess.ProcessMessages;
using Console.Core.Implementations.ExternalProcess.ProcessMessages.Messages;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.Core.Implementations.ExternalProcess
{
    internal class ConsoleProcessWrapper : IDisposable
    {
        public event Action<string> NewUserCommand;
        public bool IsRunning => _process != null && !_process.HasExited;
        private string ExecutedFilePath { get; }
        private IConsoleProcessMessageFormatter ConsoleProcessMessageFormatter { get; }

        private Process _process;
        private bool _disposed;
        
        public ConsoleProcessWrapper(string executedFilePath, [NotNull] IConsoleProcessMessageFormatter consoleProcessMessageFormatter)
        {
            if (!File.Exists(executedFilePath))
                throw new FileNotFoundException(executedFilePath);
            
            ExecutedFilePath = executedFilePath ?? throw new ArgumentNullException(nameof(executedFilePath));
            ConsoleProcessMessageFormatter = consoleProcessMessageFormatter ?? throw new ArgumentNullException(nameof(consoleProcessMessageFormatter));
        }

        public void Dispose()
        {
            _disposed = true;
            NewUserCommand = null;
            StopInternal();
        }

        public void Run()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            if (IsRunning) throw new ConsoleException("Console process already running");

            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                FileName = ExecutedFilePath,
            };
            _process = Process.Start(processStartInfo);
            if (_process != null)
            {
                _process.OutputDataReceived += ProcessOnOutputDataReceived;
            }
        }

        public Task WaitWhileStarted()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));

            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!ProcessIsRunning(_process))
                    {
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignore
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public void SendShowCommand()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            
            var message = new ShowMessage();
            SendMessage(message);
        }

        public void SendHideCommand()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            
            var message = new HideMessage();
            SendMessage(message);
        }
        
        public void SendClearCurrentCommand()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            
            var message = new ClearCurrentMessage();
            SendMessage(message);
        }

        public void SendClearAllCommand()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            
            var message = new ClearAllMessage();
            SendMessage(message);
        }

        public void NewMessages(IReadOnlyCollection<IConsoleMessage> messages)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            
            var processMessage = new NewMessagesConsoleProcessMessage(messages);
            SendMessage(processMessage);
        }

        public void Stop()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleProcessWrapper));
            StopInternal();
        }

        private void SendMessage(IConsoleProcessMessage message)
        {
            var serializedMessage = ConsoleProcessMessageFormatter.Serialize(message);
            if (serializedMessage != null)
            {
                var bufferSize = serializedMessage.Length;
                _process.StandardInput.WriteLine(bufferSize);
                _process.StandardInput.Write(serializedMessage);
            }
        }
        
        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var userCommand = e.Data;
            if (!string.IsNullOrWhiteSpace(userCommand))
            {
                NewUserCommand?.Invoke(userCommand);
            }
        }

        private void StopInternal()
        {
            var process = _process;
            _process = null;
            
            if (process != null)
            {
                process.OutputDataReceived -= ProcessOnOutputDataReceived;
                process.Kill();
            }
        }
        
        private static bool ProcessIsRunning([CanBeNull] Process process)
        {
            if (process == null) return false;
            
            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}