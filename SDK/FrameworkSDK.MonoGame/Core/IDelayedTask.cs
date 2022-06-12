using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Core
{
    public interface IDelayedTask
    {
        bool Cancelled { get; }
        void Execute(GameTime gameTime);
    }
}