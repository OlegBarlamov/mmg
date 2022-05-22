using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.MacOS.Components
{
    public class RectangleModel
    {
        public Texture2D Texture { get; set; }
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.Brown;
    }
    
    public class RectangleView : View<RectangleModel>
    {
        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            context.Draw(DataModel.Texture, DataModel.Position, new Rectangle(0, 0, DataModel.Width, DataModel.Height), DataModel.Color);
        }
    }
}