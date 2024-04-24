using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegas.Client.MacOs.Models
{
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
                DataModel.ViewModel.BoundingBox,
                DataModel.ViewModel.Color
            );

            context.Draw(_texture,
                DataModel.ViewModel.HeartBoundingBox,
                DataModel.ViewModel.HeartColor
            );
        }
    }
}