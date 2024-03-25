using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public class MonoGameGamePadDataSourceMeta : IGamePadDataSourceMeta
    {
        public string DisplayName { get; }
        public GamePadType GamePadType { get; }
        public string Identifier { get; }

        public GamePadCapabilities GamePadCapabilities { get; }

        public MonoGameGamePadDataSourceMeta(GamePadCapabilities gamePadCapabilities)
        {
            GamePadCapabilities = gamePadCapabilities;
            DisplayName = GamePadCapabilities.DisplayName;
            GamePadType = GamePadCapabilities.GamePadType;
            Identifier = gamePadCapabilities.Identifier;
        }
    }
}