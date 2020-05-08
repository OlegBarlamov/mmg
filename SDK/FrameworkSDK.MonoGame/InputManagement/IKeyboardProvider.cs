using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.InputManagement
{
    public interface IKeyboardProvider
    {
        KeyboardState Current { get; }
        KeyboardState Previous { get; }
        
        bool Key(Keys key);

        bool KeyPressedOnce(Keys key);

        bool KeyReleasedOnce(Keys key);
    }
}