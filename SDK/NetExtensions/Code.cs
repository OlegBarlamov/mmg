using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetExtensions
{
	//TODO распределить
	public static class Code
	{
		/// <summary>
		/// Determines whether type is Nullable type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if type is Nullable type; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullableType(Type type)
		{
			if (!type.IsValueType)
				return true;
			if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>)))
				return true;
			return false;
		}

		/// <summary>
		/// Converts the massive2D to massive1D.
		/// </summary>
		/// <typeparam name="T">The data type</typeparam>
		/// <param name="massive">The massive.</param>
		/// <returns></returns>
		public static T[] ConvertMassive2To1<T>(T[,] massive)
		{
			int n = massive.GetLength(0);
			int m = massive.GetLength(1);
			var data = new T[n*m];

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					data[i*m + j] = massive[i, j];

			return data;
		}

		/// <summary>
		/// Gets the name of the member.
		/// Use it as () => <c>member</c>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="memberExpression">The member expression.</param>
		/// <returns></returns>
		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			var expressionBody = (MemberExpression) memberExpression.Body;
			return expressionBody.Member.Name;
		}

		/// <summary>
		/// Gets the property by property name in type, or null if property with this name has not exist.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="onlyPublic"></param>
		/// <returns></returns>
		public static PropertyInfo GetPropertyByName(Type ownerType, string propertyName, bool onlyPublic = true)
		{
			var props = ownerType.GetProperties().ToList();
			if (!onlyPublic)
				props.AddRange(ownerType.GetProperties(BindingFlags.NonPublic |
				                                       BindingFlags.Instance));
			return props.FirstOrDefault(p => p.Name == propertyName);
		}

		/// <summary>
		/// Logic OR.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparableValue">The comparable value.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static bool Or<T>(T comparableValue, params T[] values)
		{
			return values.Contains(comparableValue);
		}

		/// <summary>
		/// Logic AND.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="comparableValue">The comparable value.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static bool And<T>(T comparableValue, params T[] values)
		{
			return values.All(value => Equals(comparableValue, value));
		}

		/// <summary>
		/// To swap the objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj1">The obj1.</param>
		/// <param name="obj2">The obj2.</param>
		public static void Swap<T>(ref T obj1, ref T obj2)
		{
			var any = obj2;
			obj2 = obj1;
			obj1 = any;
		}

		/// <summary>
		/// Gets the description of the enum value (you must specify the attribute [Description]).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			var attributes =
				(DescriptionAttribute[]) fi.GetCustomAttributes(
					typeof (DescriptionAttribute),
					false);

			if (attributes.Length > 0)
				return attributes[0].Description;
			return string.Empty;
		}

		/// <summary>
		/// Type the has attribute with specific type.
		/// This comparision only by attribute type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="attributeType">Type of the attribute.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static bool TypeHasAttribute(Type type, Type attributeType, bool inherit = false)
		{
			return type.IsDefined(attributeType, inherit);
		}

		/// <summary>
		/// Enum of type T has flag.
		/// Type T should be Enum type and has [Flags] attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		public static bool EnumHasFlag<T>(T value, T flag) where T : struct, IComparable, IFormattable, IConvertible
		{
			var type = typeof (T);
			if (!type.IsEnum)
			{
				//Common.Logger.PutError("attempt to get contains flag of non Enum type", Tags.Core);
				return false;
			}

			if (!TypeHasAttribute(type, typeof (FlagsAttribute)))
			{
				//Common.Logger.PutError("attempt to get contains flag of Enum type without Flags attribute", Tags.Core);
				return false;
			}

			bool resultValue;
			var convertedValue = ConvertToEnum(EnumTryParse<T>(value, out resultValue));
			bool resultFlag;
			var convertedFlag = ConvertToEnum(EnumTryParse<T>(flag, out resultFlag));
			if (!(resultValue && resultFlag))
			{
				//Common.Logger.PutError("error while converting instances to enum type in checking for flag function", Tags.Core);
				return false;
			}

			return convertedValue.HasFlag(convertedFlag);
		}

		/// <summary>
		/// Try get instance of the spcific enum type.
		/// </summary>
		/// <typeparam name="T">Enum type</typeparam>
		/// <param name="instance">The instance.</param>
		/// <param name="result"><c>True</c> if conversion was successful; otherwise <c>False</c></param>
		/// <returns>Converted instance</returns>
		public static T EnumTryParse<T>(object instance, out bool result)
			where T : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof (T).IsEnum)
			{
				//Common.Logger.PutError("attempt to enum try parse for non Enum type", Tags.Core);
				result = false;
				return default(T);
			}

			T converted;
			result = Enum.TryParse(instance.ToString(), out converted);
			return converted;
		}

		/// <summary>
		/// Converts to enum type.
		/// Dont't Parse! Use it like <c>(Enum)instance</c>.
		/// Returns converted value or default(Enum) value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		/// <returns>converted value or default(Enum)</returns>
		public static Enum ConvertToEnum<T>(T instance) where T : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof (T).IsEnum)
			{
				//Common.Logger.PutError("attempt to convert to enum type non enum type", Tags.Core);
				return default(Enum);
			}

			return (Enum) (object) instance;
		}
	}
}
