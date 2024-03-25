using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public class CustomGamePadDataSourceMeta : IGamePadDataSourceMeta
    {
        public string DisplayName { get; set; }
        public GamePadType GamePadType { get; set; }
        public string Identifier { get; set; }
    }
}