using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IGamepadProvider
    {
        IPlayerGamepadProvider Gamepad1 { get; }
        IPlayerGamepadProvider Gamepad2 { get; }
        IPlayerGamepadProvider Gamepad3 { get; }
        IPlayerGamepadProvider Gamepad4 { get; }

        IPlayerGamepadProvider GetGamepad(PlayerIndex index);
        
        IPlayerGamepadProvider GetGamepad(int index);
        
        GamePadDeadZone GamepadDeadZone { get; set; }

        void ActivateEmulator(IGamepadEmulator emulator);

        void DeactivateEmulator(IGamepadEmulator emulator);

    }
}