using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IGamePadDataSourceMeta
    {
        string DisplayName { get; }
        GamePadType GamePadType { get; }
        string Identifier { get; }
    }
}