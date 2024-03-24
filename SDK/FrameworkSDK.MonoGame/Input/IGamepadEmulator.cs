using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IGamepadEmulator : IPlayerGamepadProvider
    {
        void Update(GameTime gameTime);
    }
}