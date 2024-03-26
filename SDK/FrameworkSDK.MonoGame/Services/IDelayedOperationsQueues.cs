using System;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services
{
    internal interface IDelayedOperationsQueues
    {
        ConcurrentQueue<Action<GameTime>> DelayedUpdateActions { get; }
        ConcurrentQueue<Action<GameTime>> DelayedDrawActions { get; }
        ConcurrentQueue<Action> DelayedOnAppReadyActions { get; }
        void QueueOnUpdate(Action<GameTime> action);
        void QueueOnDraw(Action<GameTime> action);

        void QueueOnAppReady(Action action);
    }
}