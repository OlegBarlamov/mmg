using System.Collections.Generic;
using Atom.Client.Graphics;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atom.Client.Components
{
    public class LodStatsComponentData : ViewModel
    {
        public override string GraphicsPassName { get; set; } = GraphicsPasses.Debug;

        public SpriteFont Font;
        public Vector2 Position;
        public float LineHeight = 20f;
        public Color FontColor = Color.White;

        private readonly Dictionary<string, int> _counts = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Counts => _counts;
        public int PendingTasks { get; set; }
        public int TotalViews { get; set; }

        public void Track(string typeName, int delta)
        {
            _counts.TryGetValue(typeName, out var count);
            count += delta;
            if (count <= 0)
                _counts.Remove(typeName);
            else
                _counts[typeName] = count;
        }
    }

    public class LodStatsComponent : DrawablePrimitive<LodStatsComponentData>
    {
        public LodStatsComponent(LodStatsComponentData data) : base(data)
        {
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            var position = DataModel.Position;

            var tasksText = $"Tasks: {DataModel.PendingTasks}";
            var tasksSize = DataModel.Font.MeasureString(tasksText);
            context.DrawString(DataModel.Font, tasksText, new Vector2(position.X - tasksSize.X, position.Y),
                DataModel.PendingTasks > 20 ? Color.Red : DataModel.FontColor);
            position.Y += DataModel.LineHeight;

            var viewsText = $"Views: {DataModel.TotalViews}";
            var viewsSize = DataModel.Font.MeasureString(viewsText);
            context.DrawString(DataModel.Font, viewsText, new Vector2(position.X - viewsSize.X, position.Y), DataModel.FontColor);
            position.Y += DataModel.LineHeight;

            position.Y += DataModel.LineHeight * 0.5f;

            foreach (var kvp in DataModel.Counts)
            {
                var text = $"{kvp.Key}: {kvp.Value}";
                var size = DataModel.Font.MeasureString(text);
                context.DrawString(DataModel.Font, text, new Vector2(position.X - size.X, position.Y), DataModel.FontColor);
                position.Y += DataModel.LineHeight;
            }
        }
    }
}
