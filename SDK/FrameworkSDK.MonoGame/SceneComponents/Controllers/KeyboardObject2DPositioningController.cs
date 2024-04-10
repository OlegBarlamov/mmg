using System;
using FrameworkSDK.MonoGame.InputManagement;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;

namespace FrameworkSDK.MonoGame.SceneComponents.Controllers
{
    public class KeyboardObject2DPositioningController : KeyboardPositioningController
    {
        public ILocatable2D LocatableObject { get; }

        public KeyboardObject2DPositioningController(
            [NotNull] IInputService inputService,
            [NotNull] ILocatable2D locatableObject,
            [NotNull] KeysMap keysMap,
            bool invertY = false)
            : base(inputService, keysMap, invertY)
        {
            LocatableObject = locatableObject ?? throw new ArgumentNullException(nameof(locatableObject));
        }

        protected override void MoveDown(float speed)
        {
            LocatableObject.SetPosition(LocatableObject.Position + new Vector2(0, 1) * speed);
        }

        protected override void MoveUp(float speed)
        {
            LocatableObject.SetPosition(LocatableObject.Position + new Vector2(0, -1) * speed);
        }

        protected override void MoveRight(float speed)
        {
            LocatableObject.SetPosition(LocatableObject.Position + new Vector2(1, 0) * speed);
        }

        protected override void MoveLeft(float speed)
        {
            LocatableObject.SetPosition(LocatableObject.Position + new Vector2(-1, 0) * speed);
        }
    }
}