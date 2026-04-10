using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    public class FixedVelocityTasksProcessor : IMainUpdatesTasksProcessor, IDisposable
    {
        public int Velocity { get; }
        public int PendingTasksCount => _tasks.Count;
        private readonly Queue<IDelayedTask> _tasks = new Queue<IDelayedTask>();

        public FixedVelocityTasksProcessor(int velocity = 1)
        {
            Velocity = velocity;
        }
        
        public void EnqueueTask([NotNull] IDelayedTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _tasks.Enqueue(task);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Velocity;)
            {
                if (!_tasks.TryPeek(out var task))
                    break;

                if (task.Cancelled)
                {
                    _tasks.Dequeue();
                    continue;
                }

                if (task is BatchDelayedTask batch)
                {
                    batch.Execute();
                    i++;
                    if (batch.IsCompleted || batch.Cancelled)
                        _tasks.Dequeue();
                    continue;
                }

                _tasks.Dequeue();
                task.Execute();
                i++;
            }
        }

        public void Dispose()
        {
            _tasks.Clear();
        }
    }
}