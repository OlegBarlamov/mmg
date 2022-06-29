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

	}
}
