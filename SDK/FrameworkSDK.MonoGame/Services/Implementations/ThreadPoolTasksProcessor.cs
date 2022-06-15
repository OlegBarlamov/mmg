using System.Threading;
using FrameworkSDK.MonoGame.Core;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    public class ThreadPoolTasksProcessor : IBackgroundTasksProcessor
    {
        public void EnqueueTask(IDelayedTask task)
        {
            ThreadPool.QueueUserWorkItem(state => task.Execute());
        }
    }
}