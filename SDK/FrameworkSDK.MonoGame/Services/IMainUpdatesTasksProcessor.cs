using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services
{
    public interface IMainUpdatesTasksProcessor : IDelayedTasksProcessor
    {
        int PendingTasksCount { get; }
        void Update(GameTime gameTime);
    }
}