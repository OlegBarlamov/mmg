using System;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
    public static class ColorsExtensions
    {
        private static volatile int _globalColorIndex;
        private static readonly Color[] ColorsArray = {
            Color.Aqua,
            Color.Aquamarine,
            Color.Azure,
            Color.Beige,
            Color.Bisque,
            Color.Blue,
            Color.Brown,
            Color.Chartreuse,
            Color.Chocolate,
            Color.Coral,
            Color.Cornsilk,
            Color.CornflowerBlue,
            Color.Crimson,
            Color.Firebrick,
            Color.Fuchsia,
            Color.FloralWhite,
            Color.ForestGreen,
            Color.Honeydew,
            Color.HotPink,
            Color.Indigo,
            Color.Ivory,
            Color.IndianRed,
            Color.Khaki,
            Color.LightGray,
            Color.Lavender,
            Color.Lime,
            //..... TODO not ended
        };

        public static Color GetNextColor()
        {
             return ColorsArray[_globalColorIndex++ % ColorsArray.Length];
        }
        
        public static Color GetColorFromNumber(int number)
        {
            var count = ColorsArray.Length;
            return ColorsArray[number % count];
        }

        public static Color GetRandomPredefinedColor(Random random)
        {
            return ColorsArray[random.Next(ColorsArray.Length)];
        }
        
        public static Color GetRandomColor(Random random, int alpha = 255)
        {
            return new Color(random.Next(255), random.Next(255), random.Next(255), alpha);
        }
    }
}