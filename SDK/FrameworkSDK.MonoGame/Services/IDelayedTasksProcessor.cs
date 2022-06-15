using FrameworkSDK.MonoGame.Core;

namespace FrameworkSDK.MonoGame.Services
{
    public interface IDelayedTasksProcessor
    {
        void EnqueueTask(IDelayedTask task);
    }
}