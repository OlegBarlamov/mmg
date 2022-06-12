using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.InputManagement.Implementations
{
    internal class InputManager : IInputManager
    {
        public IInputService InputService => _inputService;
        
        private readonly InputService _inputService;

        public InputManager([NotNull] InputService inputService)
        {
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }
        
        public void Update(GameTime gameTime)
        {
            _inputService.Update(gameTime);
        }
    }
}