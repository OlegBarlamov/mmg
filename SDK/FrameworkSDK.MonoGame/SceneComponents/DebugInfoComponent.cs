using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.SceneComponents
{
    public class DebugInfoComponentData
    {
        public SpriteFont Font;

        public Vector2 Position;

        public float Tab = 10f;

        public Color FontColor = Color.White;

        public string GraphicsPassName = View.DefaultViewPassName;
    }
    
    public class DebugInfoComponent : View<DebugInfoComponentData>
    {
        private DebugInfoComponentData Data { get; }
        private IDebugInfoService DebugInfoService { get; }

        private readonly IReadOnlyDictionary<string, TimeSpan> _measures;
        private readonly IReadOnlyDictionary<string, DateTime> _timers;
        private readonly IReadOnlyDictionary<string, int> _counters;
        private readonly IReadOnlyDictionary<string, string> _labels;

        public override IReadOnlyList<string> GraphicsPassNames { get; }

        public DebugInfoComponent([NotNull] DebugInfoComponentData data, [NotNull] IDebugInfoService debugInfoService)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));

            GraphicsPassNames = new[] {data.GraphicsPassName};
            
            _measures = DebugInfoService.GetAllMeasures();
            _timers = DebugInfoService.GetAllTimers();
            _counters = DebugInfoService.GetAllCounters();
            _labels = DebugInfoService.GetAllLabels();
        }

        public override void Draw(GameTime gameTime, IDrawContext context)
        {
            base.Draw(gameTime, context);

            var position = Data.Position;
            
            var fps = (int) (TimeSpan.FromSeconds(1).TotalMilliseconds /
                      DebugInfoService.GetMeasure(nameof(IDrawable.Draw)).TotalMilliseconds);
            context.DrawString(Data.Font, $"FPS: {fps}", Data.Position, Data.FontColor);
            position += new Vector2(0, Data.Tab);
            
            foreach (var timer in _timers)
            {
                context.DrawString(Data.Font, $"{timer.Key}: {TimespanToString(DateTime.Now - timer.Value)}", position, Data.FontColor);
                position += new Vector2(0, Data.Tab);
            }

            foreach (var measure in _measures)
            {
                context.DrawString(Data.Font, $"{measure.Key}: {TimespanToString(measure.Value)}", position, Data.FontColor);
                position += new Vector2(0, Data.Tab);
            }

            foreach (var counter in _counters)
            {
                context.DrawString(Data.Font, $"{counter.Key}: {counter.Value.ToString()}", position, Data.FontColor);
                position += new Vector2(0, Data.Tab);
            }
            
            foreach (var label in _labels)
            {
                context.DrawString(Data.Font, $"{label.Key}: {label.Value}", position, Data.FontColor);
                position += new Vector2(0, Data.Tab);
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