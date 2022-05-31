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
        private const float RotationSpeed = 0.003f;
        
        private IInputService InputService { get; }
        
        public FirstPersonCameraController([NotNull] IInputService inputService, [NotNull] DirectionalCamera3D targetCamera)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            TargetCamera = targetCamera ?? throw new ArgumentNullException(nameof(targetCamera));
        }

        public void Update(GameTime gameTime)
        {
            if (InputService.Keyboard.Key(Keys.W))
            {
                var delta = GetForwardDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;
                TargetCamera.Position += delta;
                TargetCamera.Target += delta;
            }

            if (InputService.Keyboard.Key(Keys.S))
            {
                var delta = GetForwardDirection() * gameTime.ElapsedGameTime.Milliseconds * Speed;
                TargetCamera.Position -= delta;
                TargetCamera.Target -= delta;
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
                RotateLeftRight(-gameTime.ElapsedGameTime.Milliseconds * RotationSpeed);
            }
            if (InputService.Keyboard.Key(Keys.Left))
            {
                RotateLeftRight(gameTime.ElapsedGameTime.Milliseconds * RotationSpeed);
            }

            if (InputService.Keyboard.Key(Keys.Up))
            {
                RotateDownUp(gameTime.ElapsedGameTime.Milliseconds * RotationSpeed);
            }
            if (InputService.Keyboard.Key(Keys.Down))
            {
                RotateDownUp(- gameTime.ElapsedGameTime.Milliseconds * RotationSpeed);
            }

            if (InputService.Mouse.PositionDelta != Point.Zero)
            {
                RotateLeftRight(-InputService.Mouse.PositionDelta.X * RotationSpeed);
                RotateDownUp(-InputService.Mouse.PositionDelta.Y * RotationSpeed);
            }
        }

        private void RotateLeftRight(float factor)
        {
            var forward = TargetCamera.Target - TargetCamera.Position;
            forward = Vector3.Transform(forward,
                Matrix.CreateFromAxisAngle(TargetCamera.Up, factor));
            TargetCamera.Target = forward + TargetCamera.Position;
        }

        private void RotateDownUp(float factor)
        {
            var forward = TargetCamera.Target - TargetCamera.Position;
            forward = Vector3.Transform(forward,
                Matrix.CreateFromAxisAngle(GetRightDirection(), factor));
            TargetCamera.Target = forward + TargetCamera.Position;
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