using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public static class GamePadProviderExtensions
    {
        public static IPlayerGamePadProvider GetGamePad(this IGamePadProvider gamePadProvider, PlayerIndex playerIndex)
        {
            return gamePadProvider.GetGamePad((int) playerIndex);
        }

        public static void ActivateEmulator(this IGamePadProvider gamePadProvider, PlayerIndex playerIndex,
            IGamePadDataSource dataSource)
        {
            gamePadProvider.ActivateEmulator((int)playerIndex, dataSource);
        }
    }
}