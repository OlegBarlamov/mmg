using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.InputManagement;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.SceneComponents.Controllers
{
    public class KeyboardCamera2DController : KeyboardPositioningController
    {
        [NotNull] public IMutableCamera2D TargetCamera { get; }

        public KeyboardCamera2DController(
            [NotNull] IInputService inputService,
            [NotNull] IMutableCamera2D targetCamera,
            [NotNull] KeysMap keysMap,
            bool invertY = false) : base(inputService, keysMap, invertY)
        {
            TargetCamera = targetCamera ?? throw new ArgumentNullException(nameof(targetCamera));
        }

        protected override void MoveDown(float speed)
        {
            TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(0, 1) * speed * TargetCamera.GetSize().Y * 0.001f);
        }

        protected override void MoveUp(float speed)
        {
            TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(0, -1) * speed * TargetCamera.GetSize().Y * 0.001f);
        }

        protected override void MoveRight(float speed)
        {
            TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(1, 0) * speed * TargetCamera.GetSize().X * 0.001f);
        }

        protected override void MoveLeft(float speed)
        {
            TargetCamera.SetPosition(TargetCamera.GetPosition() + new Vector2(-1, 0) * speed * TargetCamera.GetSize().X * 0.001f);
        }
    }
}