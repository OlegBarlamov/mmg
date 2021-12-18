using System;
using System.Linq;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TablePlatform.Data;

namespace TablePlatform.Client
{
    public class GameScene : Scene
    {
        private GamePackage Package { get; }

        public GameScene([NotNull] GamePackage package) : base(nameof(GameScene), new TestGameModel(package))
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));

            var cardModel = ((ITableGameDescriptor) Model).InitialPosition.First();
            var viewModel = new CardViewModel(cardModel, package);
            var view = new CardView(viewModel);

            AddView(view);
        }
    }

    public class CardView : View<CardViewModel>
    {
        public CardView(CardViewModel model)
        {
            SetDataModel(model);
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            context.Draw(DataModel.Texture, DataModel.Position, Color.White);
        }

        private static Rectangle ToRectangle(IRectangle rectangle)
        {
            return new Rectangle((int)rectangle.Left, (int)rectangle.Top, (int)rectangle.Width, (int)rectangle.Height);
        }
    } 

    public class CardViewModel : IPositioned, IRectangle
    {
        public Texture2D Texture { get; }
        
        private ICanvasCard Model { get; }

        public CardViewModel([NotNull] ICanvasCard model, GamePackage gamePackage)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            Texture = gamePackage.Texture.Texture2D;
        }

        public Vector2 Position => Model.Position;
        public float Width => Model.Width;

        public float Height => Model.Height;

        public float Left => Model.Left;

        public float Right => Model.Right;

        public float Top => Model.Top;

        public float Bottom => Model.Bottom;

        public Point LeftTop => Model.LeftTop;
    }
}