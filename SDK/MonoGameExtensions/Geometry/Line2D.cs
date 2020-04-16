using System;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.Geometry
{
	/// <summary>
	/// Ax + By + C = 0
	/// </summary>
	public class Line2D : IEquatable<Line2D>
	{
		public int Left
		{
			get
			{
				if (!IsVertical)
					return int.MinValue;

				if (Math.Abs(A) < float.Epsilon)
					throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Left property");

				return Math2.Round(-C / A);
			}

		}

		public int Top
		{
			get
			{
				if (!IsHorizontal)
					return int.MinValue;

				if (Math.Abs(B) < float.Epsilon)
					throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Top property");

				return Math2.Round(-C / B);
			}
		}

		public int Width => IsVertical ? 1 : int.MaxValue;
		public int Height => IsHorizontal ? 1 : int.MaxValue;

		public Point Center
		{
			get
			{
				if (IsVertical)
				{
					if (Math.Abs(A) < float.Epsilon)
						throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Center property");

					return new Point(Math2.Round(-C / A), 0);
				}

				if (IsHorizontal)
				{
					if (Math.Abs(B) < float.Epsilon)
						throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Center property");

					return new Point(0, Math2.Round(-C / B));
				}

				if (Math.Abs(A) < float.Epsilon || Math.Abs(B) < float.Epsilon)
					throw new Exception("getting a property of the Line2D: the Line2D was vertical or horizontal but its was not definitely");

				return new Point(0, Math2.Round(-C / B));
			}
		}

		public float Rotation
		{
			get
			{
				if (IsVertical)
					return (float)Math.PI / 2;

				return (float)Math.Atan(Inclination);
			}
			set { Inclination = (float)Math.Tan(value); }
		}

		/// <summary>
		/// Gets or sets the inclination.
		/// y = kx + b. Inclination = k.
		/// </summary>
		/// <value>
		/// The inclination.
		/// </value>
		public float Inclination
		{
			get { return -A / B; }
			set { A = -value * B; }
		}

		/// <summary>
		/// Gets or sets the shift.
		/// y = kx + b. Shift = b.
		/// </summary>
		/// <value>
		/// The shift.
		/// </value>
		public float Shift
		{
			get { return -C / B; }
			set { C = -value * B; }
		}
		public bool IsVertical { get { return Math.Abs(B) < float.Epsilon; } }
		public bool IsHorizontal { get { return Math.Abs(A) < float.Epsilon; } }

		public float A;
		public float B;
		public float C;

		public Line2D(float a, float b, float c)
		{
			A = a;
			B = b;
			C = c;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Line2D"/> class.
		/// y = kx + b. k = inclination. b = shift.
		/// </summary>
		/// <param name="inclination">The inclination.</param>
		/// <param name="shift">The shift.</param>
		public Line2D(float inclination, float shift)
		{
			B = 1;
			Inclination = inclination;
			Shift = shift;
		}
		
		public static Line2D FromAngle(float angle, float shift = 0)
		{
			var inclination = (float)Math.Tan(MathHelper.ToRadians(angle));
			return new Line2D(inclination, shift);
		}

		public static Line2D FromTwoPoints(Vector2 point1, Vector2 point2)
		{
			var inclination = (point2.Y - point1.Y) / (point2.X - point1.X);
			var shift = (point1.X * point1.Y - point1.X * point2.Y) / (point2.X - point1.X) + point1.Y;

			return new Line2D(inclination, shift);
		}

		public static Line2D FromVector2(Vector2 vector)
		{
			return FromTwoPoints(new Vector2(0, 0), new Vector2(vector.X, vector.Y));
		}

		public static Line2D FromPointAngle(Vector2 point, float angle)
		{
			if (Math.Abs(MathHelper.ToDegrees(angle) - 90) < float.Epsilon)
				return new Line2D(1, 0, -point.X);

			var inclination = (float)Math.Tan(angle);
			var shift = point.Y - inclination * point.X;

			return new Line2D(inclination, shift);
		}

		public static Line2D FromNormalAndPoint(Line2D normal, Vector2 point)
		{
			return new Line2D(normal.B, -normal.A, -normal.B * point.X + normal.A * point.Y);
		}

		public bool Contains(Vector2 point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(Point point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(float x, float y)
		{
			return Math.Abs(A * x + B * y + C) < float.Epsilon;
		}

		public bool Contains(int x, int y)
		{
			return Math2.Round(A * x + B * y + C) == 0;
		}

		/// <summary>
		/// Moves location to current position + offset.
		/// </summary>
		/// <param name="offset">The offset.</param>
		public void Move(Vector2 offset)
		{
			if (!IsVertical)
			{
				float x = offset.X;
				float y = -C / B + offset.Y;

				C = -A * x - B * y;
			}
			else
				C += offset.X;

		}

		public void MoveTo(Point point)
		{
			if (!IsVertical)
			{
				float x = point.X;
				float y = point.Y;

				C = -A * x - B * y;
			}
			else
				C = point.X;
		}

		public bool Equals(Line2D other)
		{
			return Math.Abs(A - other.A) < float.Epsilon && Math.Abs(B - other.B) < float.Epsilon &&
			       Math.Abs(C - other.C) < float.Epsilon;
		}
		
		public void AddRotation(float angle)
		{
			Rotation += angle;
		}

		public void RotateTo90()
		{
			AddRotation(MathHelper.ToRadians(90));
		}

		public void RotateToMinus90()
		{
			AddRotation(MathHelper.ToRadians(-90));
		}

		public void RotateTo180()
		{
			AddRotation(MathHelper.ToRadians(180));
		}

		public void RotateToMinus180()
		{
			AddRotation(MathHelper.ToRadians(-180));
		}

		public void ResetRotation()
		{
			Rotation = 0;
		}

		public bool IsNormalTo(Line2D line)
		{
			return Math.Abs(A * line.A + B * line.B) < float.Epsilon;
		}

		public bool IsParallel(Line2D line)
		{
			return Math.Abs(A / line.A - B / line.B) < float.Epsilon;
		}

		public Vector2 GetIntersection(Line2D line)
		{
			if (IsParallel(line))
				return new Vector2(float.NaN, float.NaN);

			if (Math.Abs(line.A) < float.Epsilon)
				return line.GetIntersection(this);

			float y = (A * line.C / line.A - C) / (B - line.B * A / line.A);
			float x = -(line.B * y + line.C) / line.A;

			return new Vector2(x, y);
		}

		public bool IsIntersect(Line2D line)
		{
			return !IsParallel(line);
		}
	}
}