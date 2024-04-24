using System;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Physics._2D.Forces;
using FrameworkSDK.MonoGame.Physics2D;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Omegas.Client.MacOs.Models
{
    public class CharacterController : Controller<CharacterData>
    {
        private IInputService InputService { get; }

        private IPlayerGamePadProvider _gamePadProvider;

        private FixedForce _moveForce = new FixedForce(Vector2.Zero, 0f);
        private bool _forceApplied;

        public CharacterController([NotNull] IInputService inputService)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (_gamePadProvider.IsConnected)
            {
                var thumbSticksLeft = _gamePadProvider.ThumbSticks.Left;
                if (thumbSticksLeft != Vector2.Zero)
                {
                    thumbSticksLeft.Normalize();

                    var shift = thumbSticksLeft * new Vector2(1, -1);
                    DataModel.SetHeartRelativePosition(shift * (DataModel.Size - DataModel.HeartSize / 2));
                    _moveForce.Power = shift;
                    if (!_forceApplied)
                    {
                        _forceApplied = true;
                        ((MainScene)OwnedScene).Physics2D.ApplyForce(DataModel, _moveForce);
                    }
                }
                else
                {
                    if (_forceApplied)
                    {
                        _forceApplied = false;
                        ((MainScene)OwnedScene).Physics2D.RemoveForce(DataModel, _moveForce);
                    }
                    
                    if (DataModel.HeartRelativePosition != Vector2.Zero)
                    {
                        DataModel.SetHeartRelativePosition(Vector2.Zero);
                    }
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