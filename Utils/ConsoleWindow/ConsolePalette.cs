using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Colors = System.Windows.Media.Colors;

namespace ConsoleWindow
{
    public interface IConsolePalette
    {
        [NotNull] Brush DefaultBrush { get; }

        [CanBeNull] Brush FindBrush(LogLevel logLevel);
    }

    internal class ConsolePalette : IConsolePalette
    {
        public Brush DefaultBrush { get; }

        private readonly Dictionary<LogLevel, SolidColorBrush> _palette = new Dictionary<LogLevel, SolidColorBrush>();

        public ConsolePalette()
        {
            var whiteBrush = CreateBrush(Colors.White);
            var redBrush = CreateBrush(Colors.Red);
            var darkRed = CreateBrush(Colors.DarkRed);
            var orangeBrush = CreateBrush(Colors.Orange);
            var grayBrush = CreateBrush(Colors.LightGray);
            
            DefaultBrush = whiteBrush;

            _palette.Add(LogLevel.Critical, darkRed);
            _palette.Add(LogLevel.Debug, grayBrush);
            _palette.Add(LogLevel.Error,redBrush);
            _palette.Add(LogLevel.Information, whiteBrush);
            _palette.Add(LogLevel.None, whiteBrush);
            _palette.Add(LogLevel.Trace, whiteBrush);
            _palette.Add(LogLevel.Warning, orangeBrush);
        }

        public Brush FindBrush(LogLevel logLevel)
        {
            return _palette.TryGetValue(logLevel, out var brush) ? brush : null;
        }

        private static SolidColorBrush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}
