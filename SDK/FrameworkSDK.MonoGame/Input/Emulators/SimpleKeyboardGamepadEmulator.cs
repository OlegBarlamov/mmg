using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement.Emulators
{
    public class SimpleKeyboardGamepadEmulator : IGamePadDataSource
    {
        public IKeyboardProvider KeyboardProvider { get; }
        
        private IGamePadDataSourceMeta _meta = new CustomGamePadDataSourceMeta
        {
            DisplayName = nameof(SimpleKeyboardGamepadEmulator),
            GamePadType = GamePadType.Unknown,
            Identifier = Hash.Generate(HashType.SmallGuid).ToString()
        }; 

        public SimpleKeyboardGamepadEmulator([NotNull] IKeyboardProvider keyboardProvider)
        {
            KeyboardProvider = keyboardProvider ?? throw new ArgumentNullException(nameof(keyboardProvider));
        }
        
        public void Dispose()
        {
            
        }

        public GamePadState GetState()
        {
            var leftStick = new Vector2();
            var rightStick = new Vector2();
            
            if (KeyboardProvider.Key(Keys.Down))
            {
                leftStick += new Vector2(0, -1);
            }
            
            if (KeyboardProvider.Key(Keys.Up))
            {
                leftStick += new Vector2(0, 1);
            }
            
            if (KeyboardProvider.Key(Keys.Right))
            {
                leftStick += new Vector2(1, 0);
            }
            
            if (KeyboardProvider.Key(Keys.Left))
            {
                leftStick += new Vector2(-1, 0);
            }

            var leftTrigger = 0f;
            var rightTrigger = 0f;

            Buttons buttons = 0;

            if (KeyboardProvider.Key(Keys.A))
            {
                buttons |= Buttons.A;
            }
            if (KeyboardProvider.Key(Keys.B))
            {
                buttons |= Buttons.B;
            }
            if (KeyboardProvider.Key(Keys.X))
            {
                buttons |= Buttons.X;
            }
            if (KeyboardProvider.Key(Keys.Y))
            {
                buttons |= Buttons.Y;
            }

            var state = new GamePadState(leftStick, rightStick, leftTrigger, rightTrigger, buttons);
            

            return state;
        }

        public IGamePadDataSourceMeta GetMeta()
        {
            return _meta;
        }

        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return false;
        }
    }
}