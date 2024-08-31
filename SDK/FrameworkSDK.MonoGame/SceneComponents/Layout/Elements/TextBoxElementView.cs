using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions.Geometry;

namespace FrameworkSDK.MonoGame.SceneComponents.Layout.Elements
{
    public class TextBoxElementViewModel : LayoutUiElement
    {
        public string Text { get; set; } = "TextBox";
        
        public SpriteFont Font { get; set; }
        
        public float Rotation { get; set; }
        
        public Color Color { get; set; } = Color.Black;

        public float TextSize { get; private set; }

        public override void Arrange(RectangleF rect)
        {
            base.Arrange(rect);
            
            var size = Font.MeasureString(Text);
            TextSize = ActualRect.Width / size.X;
        }
    }

    public class TextBoxElementView : DrawablePrimitive<TextBoxElementViewModel>
    {
        public TextBoxElementView(TextBoxElementViewModel data)
            : base(data)
        {
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            if (DataModel.CheckVisible())
            {
                context.DrawString(DataModel.Font, DataModel.Text, DataModel.ActualRect.Location,
                    DataModel.Color, DataModel.Rotation, Vector2.Zero, DataModel.TextSize, SpriteEffects.None, 1f);
            }
        }
    } 
}