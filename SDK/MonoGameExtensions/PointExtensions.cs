using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
	public static class PointExtensions
	{
		public static Vector2 ToVector2(this Point point)
		{
			return new Vector2(point.X, point.Y);
		}

		public static Vector3 ToVector3FromXY(this Point point, int z)
		{
			return new Vector3(point.X, point.Y, z);
		}

		public static Vector3 ToVector3FromXZ(this Point point, int y)
		{
			return new Vector3(point.X, y, point.Y);
		}
		
		/// <summary>
		/// Determines whether the point1 more then point2. If Y is stronger coordinate then X.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		/// <returns>
		///   <c>true</c> if point1 more then point2; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPointMoreDominanteY(this Point point1, Point point2)
		{
			if (point1.Y > point2.Y)
				return true;

			if (point2.Y > point1.Y)
				return false;

			return point1.X > point2.X;
		}

		/// <summary>
		/// Determines whether the point1 more then point2. If X is stronger coordinate then Y.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		/// <returns>
		///   <c>true</c> if point1 more then point2; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPointMoreDominanteX(this Point point1, Point point2)
		{
			if (point1.X > point2.X)
				return true;

			if (point2.X > point1.X)
				return false;

			return point1.Y > point2.Y;
		}

		public static Point GetRight(this Point point, int shift = 1)
		{
			return new Point(point.X + shift, point.Y);
		}
		
		public static Point GetLeft(this Point point, int shift = 1)
		{
			return new Point(point.X - shift, point.Y);
		}
		
		public static Point GetTop(this Point point, int shift = 1)
		{
			return new Point(point.X - shift, point.Y - shift);
		}

		public static Point GetBottom(this Point point, int shift = 1)
		{
			return new Point(point.X - shift, point.Y + shift);
		}
		
		public static Point GetRightTop(this Point point, int shiftX = 1, int shiftY = 1)
		{
			return new Point(point.X + shiftX, point.Y - shiftY);
		}
		
		public static Point GetLeftTop(this Point point, int shiftX = 1, int shiftY = 1)
		{
			return new Point(point.X - shiftX, point.Y - shiftY);
		}
		
		public static Point GetRightBottom(this Point point, int shiftX = 1, int shiftY = 1)
		{
			return new Point(point.X + shiftX, point.Y + shiftY);
		}
		
		public static Point GetLeftBottom(this Point point, int shiftX = 1, int shiftY = 1)
		{
			return new Point(point.X - shiftX, point.Y + shiftY);
		}

		public static IEnumerable<Point> GetAdjusted(this Point point)
		{
			yield return point.GetTop();
			yield return point.GetRightTop();
			yield return point.GetRight();
			yield return point.GetRightBottom();
			yield return point.GetBottom();
			yield return point.GetLeftBottom();
			yield return point.GetLeft();
			yield return point.GetLeftTop();
		} 
	}
}
