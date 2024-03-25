using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IPlayerGamePadProvider : IDisposable
    {
        event Action<GamePadEventHandlerArgs> Connected;
        event Action<GamePadEventHandlerArgs> Disconnected;
        
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

        IGamePadDataSourceMeta GetGamePadMeta();

        int GetIndex();

        bool SetVibration(float leftMotor, float rightMotor);
    }
}