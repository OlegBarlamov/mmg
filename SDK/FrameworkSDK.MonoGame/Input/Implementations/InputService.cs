using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class InputService : IInputService, IUpdatable
    {
        public IKeyboardProvider Keyboard => _keyboardProvider;
        public IMouseProvider Mouse => _mouseProvider;
        public IGamePadProvider GamePads => _gamePadProvider;

        private readonly KeyboardProvider _keyboardProvider = new KeyboardProvider();
        private readonly MouseProvider _mouseProvider = new MouseProvider();
        private readonly GamePadProvider _gamePadProvider;

        public InputService([NotNull] IFrameworkLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            
            _gamePadProvider = new GamePadProvider(logger);
        }
        
        public void Update(GameTime gameTime)
        {
            _keyboardProvider.Update(gameTime);
            _mouseProvider.Update(gameTime);
            _gamePadProvider.Update(gameTime);
        }
    }
}