using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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

	    [CanBeNull] Brush FindBrush(ConsoleColor color);
	}

    internal class ConsolePalette : IConsolePalette
    {
        public Brush DefaultBrush { get; }

        private readonly Dictionary<LogLevel, SolidColorBrush> _palette = new Dictionary<LogLevel, SolidColorBrush>();
		private readonly ConcurrentDictionary<ConsoleColor, SolidColorBrush> _consoleColors = new ConcurrentDictionary<ConsoleColor, SolidColorBrush>();

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

	    public Brush FindBrush(ConsoleColor color)
	    {
		    return _consoleColors.GetOrAdd(color, CreateBrush);
	    }

	    private static SolidColorBrush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

	    private static SolidColorBrush CreateBrush(ConsoleColor color)
	    {
		    var clr = ConsoleColorToColor(color);
		    return CreateBrush(clr);
	    }

	    private static Color ConsoleColorToColor(ConsoleColor color)
	    {
		    switch (color)
		    {
			    case ConsoleColor.Black:
				    return Colors.Black;
			    case ConsoleColor.Blue:
				    return Colors.Blue;
			    case ConsoleColor.Cyan:
				    return Colors.Cyan;
			    case ConsoleColor.DarkBlue:
				    return Colors.DarkBlue;
			    case ConsoleColor.DarkGray:
				    return Colors.DarkGray;
			    case ConsoleColor.DarkGreen:
				    return Colors.DarkGreen;
			    case ConsoleColor.DarkMagenta:
				    return Colors.DarkMagenta;
			    case ConsoleColor.DarkRed:
				    return Colors.DarkRed;
			    case ConsoleColor.DarkYellow:
				    return Color.FromArgb(255, 128, 128, 0);
			    case ConsoleColor.Gray:
				    return Colors.Gray;
			    case ConsoleColor.Green:
				    return Colors.Green;
			    case ConsoleColor.Magenta:
				    return Colors.Magenta;
			    case ConsoleColor.Red:
				    return Colors.Red;
			    case ConsoleColor.White:
				    return Colors.White;
				case ConsoleColor.Yellow:
					return Colors.Yellow;
				default:
					return Colors.White;
			}
	    }
	}
}
