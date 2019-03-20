using System;
using System.Collections.Concurrent;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace ConsoleWindow.ViewModels
{
    internal class DispatcherDelayedExecutor : IDisposable
    {
        private readonly ConcurrentQueue<Action> _delayedActions = new ConcurrentQueue<Action>();
        private readonly DispatcherTimer _dispatcherTimer;
        private bool _disposed;

        private readonly object _enqueueLocker = new object();

        public DispatcherDelayedExecutor([NotNull] Dispatcher dispatcher, DispatcherPriority priority, TimeSpan executePeriod)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            _dispatcherTimer = new DispatcherTimer(priority, dispatcher)
            {
                Interval = executePeriod
            };

            _dispatcherTimer.Tick += DispatcherTimerOnTick;
        }

        public void EnqueueAction([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_enqueueLocker)
            {
                if (_disposed)
                    return;

                if (!_dispatcherTimer.IsEnabled)
                    _dispatcherTimer.Start();

                _delayedActions.Enqueue(action);
            }
        }

        public void Dispose()
        {
            lock (_enqueueLocker)
            {
                if (_disposed)
                    return;

                _disposed = true;

                _dispatcherTimer.Tick -= DispatcherTimerOnTick;
                _dispatcherTimer.Stop();
                while (_delayedActions.TryDequeue(out _))
                {

                }
            }
        }

        private void DispatcherTimerOnTick(object sender, EventArgs e)
        {
            var count = _delayedActions.Count;

            while (count > 0 && _delayedActions.TryDequeue(out var action))
            {
                count--;

                action?.Invoke();
            }

            _dispatcherTimer.IsEnabled = _delayedActions.Count > 0;
        }
    }
}
