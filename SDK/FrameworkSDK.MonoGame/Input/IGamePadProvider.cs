using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IGamePadProvider
    {
        int MaximumGamePadCount { get; set; }
        IMonoGameGamePadSettings MonoGameGamePadSettings { get; }
        IPlayerGamePadProvider GetGamePad(int playerIndex);
        void EnableGamePads();
        void DisableGamePads();
        void ActivateEmulator(int playerIndex, IGamePadDataSource emulator);
        void DeactivateEmulator(int playerIndex);

    }
}