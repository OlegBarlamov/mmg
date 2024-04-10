using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;
using MonoGameExtensions.Geometry;

namespace Omegas.Client.MacOs.Models
{
    public class CharacterData : ILocatable2D
    {
        public Vector2 Position { get; set; } = new Vector2(50, 50);

        public Vector2 HeartRelativePosition { get; set; } = Vector2.Zero;

        public float HeartSize = 10;

        public Color Color { get; } = Color.Red;

        public Color HeartColor { get; } = Color.DarkRed;

        public float Size { get; set; } = 50;

        public PlayerIndex PlayerIndex { get; } = PlayerIndex.One;
        
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }
    }

    public class CharacterDrawData
    {
        public RectangleF BoundingBox { get; set; }
        
        public RectangleF HeartBoundingBox { get; set; }
    }

    public class CharacterController : Controller<CharacterData>
    {
        public CharacterDrawData DrawData { get; private set; } = new CharacterDrawData();
        
        private IInputService InputService { get; }

        private IPlayerGamePadProvider _gamePadProvider;

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

                    DataModel.HeartRelativePosition = thumbSticksLeft * new Vector2(1, -1) * (DataModel.Size - DataModel.HeartSize / 2);
                    UpdateHeartBoundingBox();
                }
                else
                {
                    if (DataModel.HeartRelativePosition != Vector2.Zero)
                    {
                        DataModel.HeartRelativePosition = Vector2.Zero;
                        UpdateHeartBoundingBox();
                    }
                }
            }
            
            UpdateBoundingBox();
            UpdateHeartBoundingBox();
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);

            _gamePadProvider = InputService.GamePads.GetGamePad(DataModel.PlayerIndex);
            
            UpdateBoundingBox();
            UpdateHeartBoundingBox();
        }

        private void UpdateBoundingBox()
        {
            DrawData.BoundingBox = RectangleF.FromCenterAndSize(
                DataModel.Position,
                DataModel.Size);
        } 
        
        private void UpdateHeartBoundingBox()
        {
            DrawData.HeartBoundingBox = RectangleF.FromCenterAndSize(
                DataModel.Position + DataModel.HeartRelativePosition,
                DataModel.HeartSize);
        } 
    }

    public class CharacterView : View<CharacterData, CharacterController>
    {
        private GameResourcePackage ResourcePackage { get; }

        private Texture2D _texture;

        public CharacterView([NotNull] GameResourcePackage resourcePackage)
        {
            ResourcePackage = resourcePackage ?? throw new ArgumentNullException(nameof(resourcePackage));
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            _texture = ResourcePackage.Circle;
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            context.Draw(
                _texture,
                Controller.DrawData.BoundingBox,
                DataModel.Color
            );

            context.Draw(_texture,
                Controller.DrawData.HeartBoundingBox,
                DataModel.HeartColor
            );
        }
    }
}