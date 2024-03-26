using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class AppStateService : IAppStateService, IDelayedOperationsQueues
    {
        public ConcurrentQueue<Action<GameTime>> DelayedUpdateActions { get; } = new ConcurrentQueue<Action<GameTime>>();
        public ConcurrentQueue<Action<GameTime>> DelayedDrawActions { get; } = new ConcurrentQueue<Action<GameTime>>();
        public ConcurrentQueue<Action> DelayedOnAppReadyActions { get; } = new ConcurrentQueue<Action>();
        
        public bool IsRunning { get; set; }
        public bool IsTerminating { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsInitializing { get; set; }
        public bool CoreResourceLoaded { get; set; }
        public bool CoreResourceLoading { get; set; }
        public bool CoreResourceUnloading { get; set; }
        public bool IsUpdateStateActive { get; set; }
        public bool IsDrawStateActive { get; set; }
        public bool IsAppFocused { get; set; }
        
        
        public void QueueOnUpdate([NotNull] Action<GameTime> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            DelayedUpdateActions.Enqueue(action);
        }

        public void QueueOnDraw([NotNull] Action<GameTime> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            DelayedDrawActions.Enqueue(action);
        }

        public void QueueOnAppReady([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            DelayedOnAppReadyActions.Enqueue(action);
        }
    }
}