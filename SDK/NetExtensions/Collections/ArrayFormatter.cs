using System;

namespace NetExtensions.Collections
{
    public static class ArrayFormatter
    {
        public static string ToFormattedString<T>(this T[,] array)
        {
            var text = "";
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    text += array[x, y].ToString() + ' ';
                }

                if (y < height - 1)
                    text += Environment.NewLine;
            }
            return text;
        }
    }
}