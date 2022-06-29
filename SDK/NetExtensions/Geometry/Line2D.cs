using System;
using System.Drawing;
using NetExtensions.Helpers;

namespace NetExtensions.Geometry
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

				if (Math.Abs(A) < double.Epsilon)
					throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Left property");

				return MathExtended.Round(-C / A);
			}

		}

		public int Top
		{
			get
			{
				if (!IsHorizontal)
					return int.MinValue;

				if (Math.Abs(B) < double.Epsilon)
					throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Top property");

				return MathExtended.Round(-C / B);
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
					if (Math.Abs(A) < double.Epsilon)
						throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Center property");

					return new Point(MathExtended.Round(-C / A), 0);
				}

				if (IsHorizontal)
				{
					if (Math.Abs(B) < double.Epsilon)
						throw new Exception("getting a property of the Line2D: incorrect data in Line2D while get Center property");

					return new Point(0, MathExtended.Round(-C / B));
				}

				if (Math.Abs(A) < double.Epsilon || Math.Abs(B) < double.Epsilon)
					throw new Exception("getting a property of the Line2D: the Line2D was vertical or horizontal but its was not definitely");

				return new Point(0, MathExtended.Round(-C / B));
			}
		}

		public double Rotation
		{
			get
			{
				if (IsVertical)
					return Math.PI / 2;

				return Math.Atan(Inclination);
			}
			set { Inclination = Math.Tan(value); }
		}

		/// <summary>
		/// Gets or sets the inclination.
		/// y = kx + b. Inclination = k.
		/// </summary>
		/// <value>
		/// The inclination.
		/// </value>
		public double Inclination
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
		public double Shift
		{
			get { return -C / B; }
			set { C = -value * B; }
		}
		public bool IsVertical { get { return Math.Abs(B) < double.Epsilon; } }
		public bool IsHorizontal { get { return Math.Abs(A) < double.Epsilon; } }

		public double A;
		public double B;
		public double C;

		public Line2D(double a, double b, double c)
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
		public Line2D(double inclination, double shift)
		{
			B = 1;
			Inclination = inclination;
			Shift = shift;
		}
		
		public static Line2D FromAngle(double angle, double shift = 0)
		{
			var inclination = Math.Tan(MathExtended.ToRadians(angle));
			return new Line2D(inclination, shift);
		}

		public static Line2D FromTwoPoints(PointF point1, PointF point2)
		{
			var inclination = (point2.Y - point1.Y) / (point2.X - point1.X);
			var shift = (point1.X * point1.Y - point1.X * point2.Y) / (point2.X - point1.X) + point1.Y;

			return new Line2D(inclination, shift);
		}

		public static Line2D FromVector2(PointF vector)
		{
			return FromTwoPoints(new PointF(0, 0), new PointF(vector.X, vector.Y));
		}

		public static Line2D FromPointAngle(PointF point, double angle)
		{
			if (Math.Abs(angle - 90) < double.Epsilon)
				return new Line2D(1, 0, -point.X);

			var inclination = (double)Math.Tan(angle);
			var shift = point.Y - inclination * point.X;

			return new Line2D(inclination, shift);
		}

		public static Line2D FromNormalAndPoint(Line2D normal, PointF point)
		{
			return new Line2D(normal.B, -normal.A, -normal.B * point.X + normal.A * point.Y);
		}

		public bool Contains(PointF point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(Point point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(double x, double y)
		{
			return Math.Abs(A * x + B * y + C) < double.Epsilon;
		}

		public bool Contains(int x, int y)
		{
			return MathExtended.Round(A * x + B * y + C) == 0;
		}

		/// <summary>
		/// Moves location to current position + offset.
		/// </summary>
		/// <param name="offset">The offset.</param>
		public void Move(PointF offset)
		{
			if (!IsVertical)
			{
				double x = offset.X;
				double y = -C / B + offset.Y;

				C = -A * x - B * y;
			}
			else
				C += offset.X;

		}

		public void MoveTo(Point point)
		{
			if (!IsVertical)
			{
				double x = point.X;
				double y = point.Y;

				C = -A * x - B * y;
			}
			else
				C = point.X;
		}

		public bool Equals(Line2D other)
		{
			return Math.Abs(A - other.A) < double.Epsilon && Math.Abs(B - other.B) < double.Epsilon &&
			       Math.Abs(C - other.C) < double.Epsilon;
		}
		
		public void AddRotation(double angle)
		{
			Rotation += angle;
		}

		public void RotateTo90()
		{
			AddRotation(MathExtended.ToRadians(90));
		}

		public void RotateToMinus90()
		{
			AddRotation(MathExtended.ToRadians(-90));
		}

		public void RotateTo180()
		{
			AddRotation(MathExtended.ToRadians(180));
		}

		public void RotateToMinus180()
		{
			AddRotation(MathExtended.ToRadians(-180));
		}

		public void ResetRotation()
		{
			Rotation = 0;
		}

		public bool IsNormalTo(Line2D line)
		{
			return Math.Abs(A * line.A + B * line.B) < double.Epsilon;
		}

		public bool IsParallel(Line2D line)
		{
			return Math.Abs(A / line.A - B / line.B) < double.Epsilon;
		}

		public PointF GetIntersection(Line2D line)
		{
			if (IsParallel(line))
				return new PointF(float.NaN, float.NaN);

			if (Math.Abs(line.A) < double.Epsilon)
				return line.GetIntersection(this);

			double y = (A * line.C / line.A - C) / (B - line.B * A / line.A);
			double x = -(line.B * y + line.C) / line.A;

			return new PointF((float)x, (float)y);
		}

		public bool IsIntersect(Line2D line)
		{
			return !IsParallel(line);
		}
	}
}