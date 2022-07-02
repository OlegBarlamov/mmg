using System;
using System.Drawing;
using NetExtensions.Geometry;

namespace NetExtensions.Helpers
{
    public static class ArrayGenerator
    {
        public static int[,] GetGradientArray(int minValue, int maxValue, int width, int height, float angle, float offset = 0)
        {
            angle = angle % 360;
            if (angle >= 180)
            {
                Code.Swap(ref minValue, ref maxValue);
                angle = angle - 180;
            }

            var line1 = Line2D.FromPointAngle(new PointF((float)width / 2, (float)height / 2), MathExtended.ToRadians(angle));

            var top = new PointF(0, 0);
            var bot = new PointF(width - 1, height - 1);
            if (angle > 90)
            {
                top = new PointF(width - 1, 0);
                bot = new PointF(0, height - 1);
            }

            var beginPerpLine = Line2D.FromNormalAndPoint(line1, top);
            var endPerpLine = Line2D.FromNormalAndPoint(line1, bot);

            var begin = line1.GetIntersection(beginPerpLine);
            var end = line1.GetIntersection(endPerpLine);

            var distance = new PointF(begin.X, begin.Y).Sub(new PointF(end.X, end.Y));
            var distance1 = distance.AsVectorLength() * (1 - offset) / 2;
            var distance2 = distance.AsVectorLength() - distance1;

            var differenceValue = (maxValue - minValue) / 2;
            var dValue1 = differenceValue / distance1;
            var dValue2 = differenceValue / distance2;

            var arrayValues = new int[width, height];
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var currentPerpLine = Line2D.FromNormalAndPoint(line1, new PointF(x, y));
                var current = line1.GetIntersection(currentPerpLine);
					
                PointF d = begin.Sub(current);
                if (d.AsVectorLength() <= distance1)
                {
                    arrayValues[x, y] = (int)(minValue + dValue1 * d.AsVectorLength());
                    continue;
                }
                if (Math.Abs(d.AsVectorLength() - distance1) > float.Epsilon)
                {
                    arrayValues[x, y] = (int)(maxValue - dValue2 * (distance.AsVectorLength() - d.AsVectorLength()));
                    continue;
                }
                arrayValues[x, y] = (Math.Abs(distance2) < float.Epsilon) ? minValue : maxValue;
            }

            return arrayValues;
        }

        public static int[,] GetRandomArray(int minValue, int maxValue, int width, int height, Random seed)
        {
            return GetRandomArray(minValue, maxValue, width, height, seed, i => i);
        }

        public static T[,] GetRandomArray<T>(int minValue, int maxValue, int width, int height, Random seed, Func<int, T> cast)
        {
            var array = new T[width, height];
            for (int x = 0; x< width; x++)
            for (int y = 0; y < height; y++)
            {
                var value = seed.Next(minValue, maxValue + 1);
                array[x, y] = cast.Invoke(value);
            }

            return array;
        }
        
        public static int[,,] GetRandomArray(int minValue, int maxValue, int width, int height, int depth, Random seed)
        {
            return GetRandomArray(minValue, maxValue, width, height, depth, seed, i => i);
        }
        
        public static T[,,] GetRandomArray<T>(int minValue, int maxValue, int width, int height, int depth, Random seed, Func<int, T> cast)
        {
            var array = new T[width, height, depth];
            for (int x = 0; x< width; x++)
            for (int y = 0; y < height; y++)
            for (int z = 0; z < depth; z++)
            {
                var value = seed.Next(minValue, maxValue + 1);
                array[x, y, z] = cast.Invoke(value);
            }

            return array;
        }
    }
}