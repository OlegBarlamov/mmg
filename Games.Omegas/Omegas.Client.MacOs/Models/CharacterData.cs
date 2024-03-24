using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;

namespace Template.MacOs.Models
{
    public class CharacterData
    {
        public Vector2 Position { get; set; } = new Vector2(50, 50);

        public float Size { get; set; } = 50;
    }

    public class CharacterController : Controller<CharacterData>
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void OnAttached(SceneBase scene)
        {
            base.OnAttached(scene);
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
                null,
                new RectangleF(
                    DataModel.Position - new Vector2(DataModel.Size),
                    DataModel.Position + new Vector2(DataModel.Size))
            );
        }
    }
}