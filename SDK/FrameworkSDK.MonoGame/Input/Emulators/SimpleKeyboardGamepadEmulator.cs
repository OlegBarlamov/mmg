using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Emulators
{
    public class SimpleKeyboardGamepadEmulator : IGamepadEmulator
    {
        public PlayerIndex PlayerIndex { get; }
        public IKeyboardProvider KeyboardProvider { get; }
        public bool IsConnected { get; } = true;
        
        public GamePadState Current { get; }
        public GamePadState Previous { get; }
        public GamePadThumbSticks ThumbSticks => _thumbSticks;
        public GamePadTriggers Triggers { get; }
        public bool IsLeftTriggerPressed { get; }
        public bool IsRightTriggerPressed { get; }

        private GamePadThumbSticks _thumbSticks = new GamePadThumbSticks();

        public SimpleKeyboardGamepadEmulator(PlayerIndex playerIndex, [NotNull] IKeyboardProvider keyboardProvider)
        {
            PlayerIndex = playerIndex;
            KeyboardProvider = keyboardProvider ?? throw new ArgumentNullException(nameof(keyboardProvider));
        }
        
        public bool IsButtonDown(Buttons button)
        {
            throw new System.NotImplementedException();
        }

        public bool IsButtonUp(Buttons button)
        {
            throw new System.NotImplementedException();
        }

        public bool IsButtonPressedOnce(Buttons button)
        {
            throw new System.NotImplementedException();
        }

        public bool IsButtonReleasedOnce(Buttons button)
        {
            throw new System.NotImplementedException();
        }

        public GamePadCapabilities GetGamePadCapabilities()
        {
            throw new System.NotImplementedException();
        }

        public PlayerIndex GetIndex()
        {
            return PlayerIndex;
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            throw new System.NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            var leftStick = new Vector2();
            var rightStick = new Vector2();
            
            if (KeyboardProvider.Key(Keys.Down))
            {
                leftStick += new Vector2(0, 1);
            }
            
            if (KeyboardProvider.Key(Keys.Up))
            {
                leftStick += new Vector2(0, -1);
            }
            
            if (KeyboardProvider.Key(Keys.Right))
            {
                leftStick += new Vector2(1, 0);
            }
            
            if (KeyboardProvider.Key(Keys.Left))
            {
                leftStick += new Vector2(-1, 0);
            }
            
            _thumbSticks = new GamePadThumbSticks(leftStick, rightStick);
        }
    }
}