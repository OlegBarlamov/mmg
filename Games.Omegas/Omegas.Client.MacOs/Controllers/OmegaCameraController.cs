using System;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Omegas.Client.MacOs.Models;

namespace Omegas.Client.MacOs.Controllers
{
    public class OmegaCameraController : Controller
    {
        private PlayerData Player { get; }
        private IMutableCamera2D Camera { get; }

        private const float MaxCameraOffset = 400f;
        private Vector2 _currentCameraOffset;

        public OmegaCameraController([NotNull] PlayerData player, [NotNull] IMutableCamera2D camera)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currentCameraOffset = Player.CameraOffsetDirection * MaxCameraOffset;
            
            Camera.CenterTo(Player.Position + _currentCameraOffset);
        }
    }
}