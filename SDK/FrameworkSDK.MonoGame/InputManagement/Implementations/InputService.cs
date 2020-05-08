using Microsoft.Xna.Framework;
using IUpdateable = FrameworkSDK.MonoGame.Basic.IUpdateable;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class InputService : IInputService, IUpdateable
    {
        public IKeyboardProvider Keyboard => _keyboardProvider;
        public IMouseProvider Mouse => _mouseProvider;

        private readonly KeyboardProvider _keyboardProvider = new KeyboardProvider();
        private readonly MouseProvider _mouseProvider = new MouseProvider();
        
        public void Update(GameTime gameTime)
        {
            _keyboardProvider.Update(gameTime);
            _mouseProvider.Update(gameTime);
        }
    }
}