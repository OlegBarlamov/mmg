using System;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions
{
    public static class Math2
	{
		/// <summary>
		/// Determines whether value in interval (without bounds).
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if value in interval otherwise, <c>false</c>.
		/// </returns>
		public static bool InInterval(double left, double right, double value)
		{
			return value > left && value < right;
		}

		/// <summary>
		/// Determines whether value in segment (with bounds).
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if value in segment otherwise, <c>false</c>.
		/// </returns>
		public static bool InSegment(double left, double right, double value)
		{
			return value >= left && value <= right;
		}

		/// <summary>
		/// Determines whether value in interval (with left bound).
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if value in interval otherwise, <c>false</c>.
		/// </returns>
		public static bool InIntervalLeft(double left, double right, double value)
		{
			return value >= left && value < right;
		}

		/// <summary>
		/// Determines whether value in interval (with right bound).
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///   <c>true</c> if value in interval otherwise, <c>false</c>.
		/// </returns>
		public static bool InIntervalRight(double left, double right, double value)
		{
			return value > left && value <= right;
		}

		/// <summary>
		/// Determines whether value in interval (without bounds), with error equal epsilon.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <param name="value">The value.</param>
		/// <param name="epsilon">The epsilon.</param>
		/// <returns>
		///   <c>true</c> if value in interval otherwise, <c>false</c>.
		/// </returns>
		public static bool InInterval(double left, double right, double value, double epsilon)
		{
			left -= epsilon / 2;
			right += epsilon / 2;
			return value > left && value < right;
		}

		/// <summary>
		/// Returns value to the segment.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		public static void ReturnToSegment(ref int value, int left, int right)
		{
			if (value < left)
				value = left;
			if (value > right)
				value = right;
		}

		/// <summary>
		/// Returns value to the segment.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		public static void ReturnToSegment(ref float value, float left, float right)
		{
			if (value < left)
				value = left;
			if (value > right)
				value = right;
		}

		/// <summary>
		/// Determines whether the point1 more then point2. If Y is stronger coordinate then X.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		/// <returns>
		///   <c>true</c> if point1 more then point2; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPointMoreDominanteY(Point point1, Point point2)
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
		public static bool IsPointMoreDominanteX(Point point1, Point point2)
		{
			if (point1.X > point2.X)
				return true;

			if (point2.X > point1.X)
				return false;

			return point1.Y > point2.Y;
		}

		/// <summary>
		/// Get maximum from two values.
		/// </summary>
		/// <param name="a">a.</param>
		/// <param name="b">b.</param>
		/// <returns>Maximum</returns>
		public static int Max(int a, int b)
		{
			if (a > b)
				return a;

			return b;
		}

		/// <summary>
		/// Gets the maximum from two values.
		/// </summary>
		/// <param name="a">a.</param>
		/// <param name="b">b.</param>
		/// <returns>Maximum</returns>
		public static float Max(float a, float b)
		{
			if (a > b)
				return a;

			return b;
		}

		/// <summary>
		/// Gets the minimum from two values.
		/// </summary>
		/// <param name="a">a.</param>
		/// <param name="b">b.</param>
		/// <returns>Minimum</returns>
		public static int Min(int a, int b)
		{
			if (a < b)
				return a;

			return b;
		}

		/// <summary>
		/// Gets the minimum from two values.
		/// </summary>
		/// <param name="a">a.</param>
		/// <param name="b">b.</param>
		/// <returns>Minimum</returns>
		public static float Min(float a, float b)
		{
			if (a < b)
				return a;

			return b;
		}

		/// <summary>
		/// Rounds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static int Round(float value)
		{
			return Round((double)value);
		}

		/// <summary>
		/// Rounds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static int Round(double value)
		{
			return (int)Math.Round(value);
		}
	}
}