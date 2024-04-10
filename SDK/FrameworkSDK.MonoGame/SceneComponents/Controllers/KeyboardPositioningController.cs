using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.SceneComponents.Controllers
{
    public abstract class KeyboardPositioningController : Controller
    {
        public class KeysMap
        {
            public Keys LeftKey { get; set; } = Keys.Left;
            public Keys RightKey { get; set; } = Keys.Right;
            public Keys UpKey { get; set; } = Keys.Up;
            public Keys DownKey { get; set; } = Keys.Down;
            
            public Keys? ShiftKey { get; set; } = null;
        }

        public bool InvertY { get; }
        public float Speed { get; set; } = 1f;
        private readonly int _invertMultiplication;

        [NotNull] private IInputService InputService { get; }

        private readonly KeysMap _keysMap;

        public KeyboardPositioningController(
            [NotNull] IInputService inputService,
            [NotNull] KeysMap keysMap,
            bool invertY = false)
        {
            _keysMap = keysMap ?? throw new ArgumentNullException(nameof(keysMap));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            InvertY = invertY;
            _invertMultiplication = InvertY ? -1 : 1;
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var targetSpeed = Speed;
            if (_keysMap.ShiftKey != null && InputService.Keyboard.Key(_keysMap.ShiftKey.Value))
            {
                targetSpeed *= 2;
            }
            if (InputService.Keyboard.Key(_keysMap.DownKey))
            {
                MoveDown(_invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            if (InputService.Keyboard.Key(_keysMap.UpKey))
            {
                MoveUp(_invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            if (InputService.Keyboard.Key(_keysMap.RightKey))
            {
                MoveRight(_invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            if (InputService.Keyboard.Key(_keysMap.LeftKey))
            {
                MoveLeft(_invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        protected abstract void MoveDown(float speed);
        
        protected abstract void MoveUp(float speed);
        
        protected abstract void MoveRight(float speed);
        
        protected abstract void MoveLeft(float speed);
    }
}