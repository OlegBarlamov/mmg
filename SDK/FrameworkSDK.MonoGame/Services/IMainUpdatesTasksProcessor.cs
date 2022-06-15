using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services
{
    public interface IMainUpdatesTasksProcessor : IDelayedTasksProcessor
    {
        void Update(GameTime gameTime);
    }
}