using System.Collections.Generic;

namespace MonoGameExtensions.DataStructures
{
    public class FlatAdjusting<T>
    {
        public T Top { get; set; }
        public T RightTop { get; set; }
        public T Right { get; set; }
        public T RightBottom { get; set; }
        public T Bottom { get; set; }
        public T LeftBottom { get; set; }
        public T Left { get; set; }
        public T LeftTop { get; set; }
        public T Center { get; set; }

        public IEnumerable<T> IterateAdjusting()
        {
            yield return Top;
            yield return RightTop;
            yield return Right;
            yield return RightBottom;
            yield return Bottom;
            yield return LeftBottom;
            yield return Left;
            yield return LeftTop;
        }

        public IEnumerable<T> IterateWithCenter()
        {
            yield return Top;
            yield return RightTop;
            yield return Right;
            yield return RightBottom;
            yield return Bottom;
            yield return LeftBottom;
            yield return Left;
            yield return LeftTop;
            yield return Center;
        }
    }
}