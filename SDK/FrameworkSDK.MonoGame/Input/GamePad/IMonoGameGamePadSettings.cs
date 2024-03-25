using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IMonoGameGamePadSettings
    {
        GamePadDeadZone GamepadDeadZone { get; set; }
    }
}