using System;
using System.Threading;
using System.Threading.Tasks;

namespace Console.Core.Implementations.Terminal
{
    public class TerminalConsoleCommandListener : IDisposable
    {
        public event Action<string> Command;

        private int ListeningPeriod { get; }
        
        private Task _task;
        private readonly object _locker = new object();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public TerminalConsoleCommandListener():this(100)
        {
        }
        
        public TerminalConsoleCommandListener(int listeningPeriod)
        {
            ListeningPeriod = listeningPeriod;
        }

        public void StartListen()
        {
            if (_task != null)
                return;
            
            lock (_locker)
            {
                if (_task == null)
                    _task = Task.Factory.StartNew(ListeningFunction, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        private void ListeningFunction()
        {
            while (!_cts.IsCancellationRequested)
            {
                var line = System.Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(line))
                    Command?.Invoke(line);
                
                Thread.Sleep(ListeningPeriod);
            }
        }

        public void Dispose()
        {
            using (_cts)
            {
                _cts.Cancel();
            }
            
            Command = null;
        }
    }
}