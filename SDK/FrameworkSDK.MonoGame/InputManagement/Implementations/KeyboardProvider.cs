using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IUpdateable = FrameworkSDK.MonoGame.Basic.IUpdateable;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    public class KeyboardProvider : IKeyboardProvider, IUpdateable
    {
        public KeyboardState Current { get; private set; } = Keyboard.GetState();
        public KeyboardState Previous { get; private set; }= Keyboard.GetState();

        public bool Key(Keys key)
        {
            return IsPressed(Current[key]);
        }

        public bool KeyPressedOnce(Keys key)
        {
            return IsPressed(Current[key]) && IsReleased(Previous[key]);
        }

        public bool KeyReleasedOnce(Keys key)
        {
            return IsReleased(Current[key]) && IsPressed(Previous[key]);
        }
        
        public void Update(GameTime gameTime)
        {
            Previous = Current;
            Current = Keyboard.GetState();
        }

        private static bool IsPressed(KeyState keyState)
        {
            return keyState == KeyState.Down;
        }

        private static bool IsReleased(KeyState keyState)
        {
            return keyState == KeyState.Up;
        }
    }
}