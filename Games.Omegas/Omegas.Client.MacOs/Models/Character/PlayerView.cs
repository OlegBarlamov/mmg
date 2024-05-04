using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegas.Client.MacOs.Models
{
    public class PlayerView : View<PlayerData, PlayerController>
    {
        private GameResourcePackage ResourcePackage { get; }
        [NotNull] public ICamera2DProvider Camera2DProvider { get; }

        private Texture2D _texture;

        public PlayerView([NotNull] GameResourcePackage resourcePackage, [NotNull] ICamera2DProvider camera2DProvider)
        {
            ResourcePackage = resourcePackage ?? throw new ArgumentNullException(nameof(resourcePackage));
            Camera2DProvider = camera2DProvider ?? throw new ArgumentNullException(nameof(camera2DProvider));
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
            
            _texture = ResourcePackage.Circle;
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            if (Camera2DProvider.GetActiveCamera().IsRectangleVisible(DataModel.ViewModel.BoundingBox))
            {
                context.Draw(
                    _texture,
                    DataModel.ViewModel.BoundingBox,
                    DataModel.ViewModel.Color
                );

                context.Draw(_texture,
                    DataModel.PlayerViewModel.HeartBoundingBox,
                    DataModel.PlayerViewModel.HeartColor
                );
            }
        }
    }
}