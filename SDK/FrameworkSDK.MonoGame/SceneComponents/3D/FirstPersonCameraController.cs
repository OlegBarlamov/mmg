using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera3D;
using FrameworkSDK.MonoGame.InputManagement;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class FirstPersonCameraController : IUpdatable
    {
        public DirectionalCamera3D TargetCamera { get; }

        private const float Speed = 0.01f;
        
        private IInputService InputService { get; }
        
        public FirstPersonCameraController([NotNull] IInputService inputService, [NotNull] DirectionalCamera3D targetCamera)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            TargetCamera = targetCamera ?? throw new ArgumentNullException(nameof(targetCamera));
        }

        public void Update(GameTime gameTime)
        {
            if (InputService.Keyboard.Key(Keys.Up) || InputService.Keyboard.Key(Keys.W))
            {
                TargetCamera.Position += GetForwardDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;
            }

            if (InputService.Keyboard.Key(Keys.Down) || InputService.Keyboard.Key(Keys.S))
            {
                TargetCamera.Position -= GetForwardDirection()* gameTime.ElapsedGameTime.Milliseconds * Speed;
            }
            
            if (InputService.Keyboard.Key(Keys.D))
            {
                var delta = GetRightDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;
                TargetCamera.Position += delta;
                TargetCamera.Target += delta;
            }
            
            if (InputService.Keyboard.Key(Keys.A))
            {
                var delta = GetRightDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;
                TargetCamera.Position -= delta;
                TargetCamera.Target -= delta;
            }

            if (InputService.Keyboard.Key(Keys.Right))
            {
                TargetCamera.Position += GetRightDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;;
            }
            if (InputService.Keyboard.Key(Keys.Left))
            {
                TargetCamera.Position -= GetRightDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;;
            }
        }

        private Vector3 GetForwardDirection()
        {
            var direction = TargetCamera.Target - TargetCamera.Position;
            direction.Normalize();
            return direction;
        }

        private Vector3 GetRightDirection()
        {
            var direction = TargetCamera.Target - TargetCamera.Position;
            var right = Vector3.Cross(direction, Vector3.Up);
            right.Normalize();
            return right;
        }
    }
}