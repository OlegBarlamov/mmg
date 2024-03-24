using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class PlayerGamepadProvider : IPlayerGamepadProvider
    {
        public bool IsConnected => Current.IsConnected;
        public GamePadState Current { get; set; } = new GamePadState();
        public GamePadState Previous { get; set; } = new GamePadState();

        public GamePadThumbSticks ThumbSticks => Current.ThumbSticks;

        public GamePadTriggers Triggers => Current.Triggers;

        public bool IsLeftTriggerPressed => Triggers.Left > 0;

        public bool IsRightTriggerPressed => Triggers.Right > 0;
        
        private PlayerIndex Index { get; }

        public void UpdateState(GamePadState newState)
        {
            Previous = Current;
            Current = newState;
        }
        
        public PlayerGamepadProvider(PlayerIndex index)
        {
            Index = index;
        }
        
        public PlayerIndex GetIndex()
        {
            return Index;
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return GamePad.SetVibration(Index, leftMotor, rightMotor);
        }

        public bool IsButtonDown(Buttons button)
        {
            return Current.IsButtonDown(button);
        }
        
        public bool IsButtonUp(Buttons button)
        {
            return Current.IsButtonUp(button);
        }

        public bool IsButtonPressedOnce(Buttons button)
        {
            return IsButtonDown(button) && Previous.IsButtonUp(button);
        }

        public bool IsButtonReleasedOnce(Buttons button)
        {
            return IsButtonUp(button) && Previous.IsButtonDown(button);
        }

        public GamePadCapabilities GetGamePadCapabilities()
        {
            return GamePad.GetCapabilities(Index);
        }
    }
}