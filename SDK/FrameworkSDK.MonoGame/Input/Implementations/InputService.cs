using FrameworkSDK.MonoGame.Basic;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class InputService : IInputService, IUpdatable
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