using System;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class DrawVectorComponentDataModel : ViewModel
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; } = Color.White;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateRectangles();
            }
        }

        public float Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                UpdateRectangles();
            }
        }

        
        public Vector2 Vector
        {
            get => _vector;
            set
            {
                _vector = value;
                UpdateRectangles();
            }
        }

        internal Rectangle MainLineRectangle;
        internal float MainRectangleRotation;
        internal Rectangle LeftRectangle;
        internal float LeftRectangleRotation;
        internal Rectangle RightRectangle;
        internal float RightRectangleRotation;

        private Vector2 _position;
        private Vector2 _vector;
        private float _thickness;

        public DrawVectorComponentDataModel(Vector2 position, Vector2 vector, float thickness = 1f)
        {
            _position = position;
            _vector = vector;
            _thickness = thickness;
            
            UpdateRectangles();
        }
        
        private void UpdateRectangles()
        {
            var length = _vector.Length();
            
            MainLineRectangle = new Rectangle(_position.ToPoint(), new Point((int)length, (int)_thickness));
            MainRectangleRotation = - (float) Math.Atan2(_vector.Y, _vector.X);
            
            // LeftRectangle = new Rectangle((_position + _vector).ToPoint(), new Point((int)(length * 0.5f), (int)_thickness));
            // LeftRectangleRotation = - (float) Math.Atan2(_vector.Y, _vector.X);

            // Vector2.Transform(Vector2.One, Matrix.CreateRotationX(MainRectangleRotation));
            // RightRectangle = new Rectangle((_position + _vector).ToPoint(), new Point((int)(length * 0.1f), (int)_thickness));
        }
    }

    public class DrawVectorComponent : DrawablePrimitive<DrawVectorComponentDataModel>
    {
        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);
            
            context.Draw(DataModel.Texture, DataModel.MainLineRectangle, null, DataModel.Color, DataModel.MainRectangleRotation, Vector2.Zero, SpriteEffects.None, 1f);
            // context.Draw(DataModel.Texture, DataModel.LeftRectangle, null, DataModel.Color, DataModel.LeftRectangleRotation, Vector2.Zero, SpriteEffects.None, 1f);
            // context.Draw(DataModel.Texture, DataModel.RightRectangle, null, DataModel.Color, DataModel.RightRectangleRotation, Vector2.Zero, SpriteEffects.None, 1f);
        }

        public DrawVectorComponent(DrawVectorComponentDataModel data) : base(data)
        {
            
        }
    }
}