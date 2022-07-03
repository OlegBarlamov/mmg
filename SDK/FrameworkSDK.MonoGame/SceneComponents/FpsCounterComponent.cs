using System;
using System.Threading;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class FpsCounterComponentData : ViewModel
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public SpriteFont Font { get; set; }
        public Color Color { get; set; } = Color.White;

        public string Label { get; set; } = "FPS: ";

        public int FpsValue { get; private set; }

        private int _framesCounter;
        private double _timeSpent;
        private DateTime _lastTime;

        internal void OnDraw(GameTime gameTime)
        {
            _framesCounter++;
            _timeSpent += (DateTime.Now - _lastTime).TotalMilliseconds;
            _lastTime = DateTime.Now;
            if (_timeSpent > 2000)
            {
                _timeSpent = 0;
                _framesCounter = 0;
            }
            else
            {
                FpsValue = (int)(_framesCounter / (_timeSpent / 1000));
            }
        }
    }
    
    public class FpsCounterComponent : DrawablePrimitive<FpsCounterComponentData>
    {
        public FpsCounterComponent(FpsCounterComponentData data)
            : base(data)
        {
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            DataModel.OnDraw(gameTime);
            
            base.Draw(gameTime, context);
            
            context.DrawString(DataModel.Font, $"{DataModel.Label}{DataModel.FpsValue.ToString()}", DataModel.Position, DataModel.Color);
        }
    }
}