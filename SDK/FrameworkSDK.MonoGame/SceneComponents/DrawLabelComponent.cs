using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class DrawLabelComponentDataModel
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public SpriteFont Font { get; set; }
        public string Text { get; set; } = "Label";
        public Color Color { get; set; } = Color.White;
    }

    public class DrawLabelComponent : View<DrawLabelComponentDataModel>
    {
        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            context.DrawString(DataModel.Font, DataModel.Text, DataModel.Position, DataModel.Color);
        }
    }
}