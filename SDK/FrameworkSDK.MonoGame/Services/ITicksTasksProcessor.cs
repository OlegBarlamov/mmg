using FrameworkSDK.MonoGame.Core;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Services
{
    public interface ITicksTasksProcessor
    {
        void EnqueueTask(IDelayedTask task);

        void Update(GameTime gameTime);
    }
}