using System;

namespace NetExtensions.Helpers
{
    public static class MathExtended
	{
		public static double ToRadians(double degrees)
		{
			return degrees * Math.PI / 180;
		}
		
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
		
		public static float Max(params float[] values)
		{
			float max = float.MinValue;
			foreach (var value in values)
			{
				if (value > max)
					max = value;
			}
			return max;
		}
		
		public static float Min(params float[] values)
		{
			float min = float.MaxValue;
			foreach (var value in values)
			{
				if (value < min)
					min = value;
			}
			return min;
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

		public static int Sqr(int value)
		{
			return value * value;
		}
		
		public static float Sqr(float value)
		{
			return value * value;
		}
		
		public static double Sqr(double value)
		{
			return value * value;
		}
	}
}