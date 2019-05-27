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

	}
}
