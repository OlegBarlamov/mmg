using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.SceneComponents.Controllers
{
    public class KeyboardCamera2DController : Controller
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
        
        [NotNull] public IMutableCamera2D TargetCamera { get; }
        
        [NotNull] private IInputService InputService { get; }

        private readonly KeysMap _keysMap;

        public KeyboardCamera2DController(
            [NotNull] IInputService inputService,
            [NotNull] IMutableCamera2D targetCamera,
            [NotNull] KeysMap keysMap,
            bool invertY = false)
        {
            _keysMap = keysMap ?? throw new ArgumentNullException(nameof(keysMap));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            TargetCamera = targetCamera ?? throw new ArgumentNullException(nameof(targetCamera));
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
                TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(0, 1) * _invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds * TargetCamera.GetSize().Y * 0.001f);
            }
            if (InputService.Keyboard.Key(_keysMap.UpKey))
            {
                TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(0, -1) * _invertMultiplication * targetSpeed * gameTime.ElapsedGameTime.Milliseconds * TargetCamera.GetSize().Y * 0.001f);
            }
            if (InputService.Keyboard.Key(_keysMap.RightKey))
            {
                TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(1, 0) * targetSpeed * gameTime.ElapsedGameTime.Milliseconds * TargetCamera.GetSize().X * 0.001f);
            }
            if (InputService.Keyboard.Key(_keysMap.LeftKey))
            {
                TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(-1, 0) * targetSpeed * gameTime.ElapsedGameTime.Milliseconds * TargetCamera.GetSize().X * 0.001f);
            }
        }
    }
}