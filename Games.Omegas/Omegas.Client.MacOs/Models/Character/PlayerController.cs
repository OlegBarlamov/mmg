using System;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Omegas.Client.MacOs.Models.SphereObject;
using Omegas.Client.MacOs.Services;

namespace Omegas.Client.MacOs.Models
{
    public class PlayerController : SphereObjectGenericController<PlayerData>
    {
        private IInputService InputService { get; }
        public OmegaGameService OmegaGameService { get; }

        private IPlayerGamePadProvider _gamePadProvider;

        public PlayerController([NotNull] IInputService inputService, [NotNull] OmegaGameService omegaGameService) : base(omegaGameService)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            OmegaGameService = omegaGameService ?? throw new ArgumentNullException(nameof(omegaGameService));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (_gamePadProvider.IsConnected)
            {
                UpdateLeftThumbStick();

                if (_gamePadProvider.IsButtonPressedOnce(Buttons.A) && _gamePadProvider.ThumbSticks.Left != Vector2.Zero)
                {
                    var shift = Vector2.Normalize(_gamePadProvider.ThumbSticks.Left * new Vector2(1, -1));
                    var bulletObject = OmegaGameService.CreateBulletWorkpiece(DataModel, shift, Math.Max(2, DataModel.Size / 50));
                    // TODO Giving the right velocity should be responsibility of the service
                    OmegaGameService.ReleaseBullet(DataModel, bulletObject, DataModel.Velocity + shift * 20f);
                }
            }
        }

        private void UpdateLeftThumbStick()
        {
            var thumbSticksLeft = _gamePadProvider.ThumbSticks.Left;
            if (thumbSticksLeft != Vector2.Zero)
            {
                thumbSticksLeft.Normalize();

                var shift = thumbSticksLeft * new Vector2(1, -1);
                DataModel.SetHeartRelativePosition(shift * (DataModel.Size - DataModel.HeartSize / 2));
                //DataModel.ControllerForce.Power = shift;
            }
            else
            {
                DataModel.ControllerForce.Power = Vector2.Zero;

                if (DataModel.HeartRelativePosition != Vector2.Zero)
                {
                    DataModel.SetHeartRelativePosition(Vector2.Zero);
                }
            }
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            _gamePadProvider = InputService.GamePads.GetGamePad(DataModel.PlayerIndex);
        }
    }
}