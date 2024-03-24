using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IPlayerGamepadProvider
    {
        bool IsConnected { get; }
        
        GamePadState Current { get; }
        GamePadState Previous { get; }
        
        GamePadThumbSticks ThumbSticks { get; }
        
        GamePadTriggers Triggers { get; }
        
        bool IsLeftTriggerPressed { get; }
        bool IsRightTriggerPressed { get; }

        bool IsButtonDown(Buttons button);

        bool IsButtonUp(Buttons button);

        bool IsButtonPressedOnce(Buttons button);

        bool IsButtonReleasedOnce(Buttons button);

        GamePadCapabilities GetGamePadCapabilities();

        PlayerIndex GetIndex();

        bool SetVibration(float leftMotor, float rightMotor);
    }
}