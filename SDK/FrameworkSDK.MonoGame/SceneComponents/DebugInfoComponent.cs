using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline.Presets;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class DebugInfoComponentData : ViewModel
    {
        public override string GraphicsPassName { get; set; } =
            GraphicsPipeline2DDrawingPreset.PipelineActions.DrawDebugUI;
        
        public SpriteFont Font;

        public Vector2 Position;

        public float Tab = 10f;

        public Color FontColor = Color.White;
    }
    
    public class DebugInfoComponent : DrawablePrimitive<DebugInfoComponentData>
    {
        private IDebugInfoService DebugInfoService { get; }

        private readonly IReadOnlyDictionary<string, TimeSpan> _measures;
        private readonly IReadOnlyDictionary<string, DateTime> _timers;
        private readonly IReadOnlyDictionary<string, int> _counters;
        private readonly IReadOnlyDictionary<string, string> _labels;

        public DebugInfoComponent([NotNull] DebugInfoComponentData data, [NotNull] IDebugInfoService debugInfoService) : base(data)
        {
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));

            _measures = DebugInfoService.GetAllMeasures();
            _timers = DebugInfoService.GetAllTimers();
            _counters = DebugInfoService.GetAllCounters();
            _labels = DebugInfoService.GetAllLabels();
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            var position = DataModel.Position;
            
            // Potential draws per second. ~ Indicates potential performance capacity 
            var dps = (int) (TimeSpan.FromSeconds(1).TotalMilliseconds /
                      DebugInfoService.GetMeasure(nameof(IDrawable.Draw)).TotalMilliseconds);
            context.DrawString(DataModel.Font, $"dps: ~{dps}", DataModel.Position, DataModel.FontColor);
            position += new Vector2(0, DataModel.Tab);
            
            foreach (var timer in _timers)
            {
                context.DrawString(DataModel.Font, $"{timer.Key}: {TimespanToString(DateTime.Now - timer.Value)}", position, DataModel.FontColor);
                position += new Vector2(0, DataModel.Tab);
            }

            foreach (var measure in _measures)
            {
                context.DrawString(DataModel.Font, $"{measure.Key}: {TimespanToString(measure.Value)}", position, DataModel.FontColor);
                position += new Vector2(0, DataModel.Tab);
            }

            foreach (var counter in _counters)
            {
                context.DrawString(DataModel.Font, $"{counter.Key}: {counter.Value.ToString()}", position, DataModel.FontColor);
                position += new Vector2(0, DataModel.Tab);
            }
            
            foreach (var label in _labels)
            {
                context.DrawString(DataModel.Font, $"{label.Key}: {label.Value}", position, DataModel.FontColor);
                position += new Vector2(0, DataModel.Tab);
            }
        }

        private static string TimespanToString(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.FromMilliseconds(1))
                return $"{timeSpan.Ticks.ToString()} ticks";
            
            if (timeSpan < TimeSpan.FromSeconds(1))
                return $"{timeSpan.Milliseconds.ToString()} ms";

            if (timeSpan < TimeSpan.FromMinutes(1))
                return timeSpan.ToString("ss\\.ffff");

            return timeSpan.ToString("g");
        }
    }
}