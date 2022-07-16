using System;
using System.Threading;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Core;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    public class ThreadPoolTasksProcessor : IBackgroundTasksProcessor
    {
        private ModuleLogger Logger { get; }
        
        public ThreadPoolTasksProcessor(IFrameworkLogger logger)
        {
            Logger = new ModuleLogger(logger, LogCategories.Operations);
        }
        
        public void EnqueueTask(IDelayedTask task)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    task.Execute();
                }
                catch (Exception e)
                {
                    Logger.Error("Error while executing an operation in background thread", e);
                }
            });
        }
    }
}