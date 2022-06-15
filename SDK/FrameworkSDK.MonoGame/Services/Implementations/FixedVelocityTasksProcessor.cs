using System;
using System.Collections.Concurrent;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    public class FixedVelocityTasksProcessor : IMainUpdatesTasksProcessor, IDisposable
    {
        public int Velocity { get; }
        private readonly ConcurrentQueue<IDelayedTask> _tasks = new ConcurrentQueue<IDelayedTask>();

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
            for (int i = 0; i < Velocity; i++)
            {
                if (_tasks.TryDequeue(out var task))
                {
                    if (!task.Cancelled)
                        task.Execute();
                }
                else
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            while (_tasks.TryDequeue(out _))
            {
            }
        }
    }
}