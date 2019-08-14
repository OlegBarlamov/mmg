using System;
using System.Linq;
using FrameworkSDK.Game;
using JetBrains.Annotations;

namespace FrameworkSDK.Common
{
	public class Hash : IEquatable<Hash>
	{
		[NotNull] public string Value { get; }

		private Hash(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException(nameof(value));

			Value = value;
		}

		public static Hash Generate(HashType hashType)
		{
			switch (hashType)
			{
				case HashType.Number:
					return new Hash(GetNumber(5));
				case HashType.BigNumber:
					return new Hash(GetNumber(10));
				case HashType.Guid:
					return new Hash(GetGuid(5));
				case HashType.SmallGuid:
					return new Hash(GetGuid(1));
				default:
					throw new ArgumentOutOfRangeException(nameof(hashType), hashType, null);
			}
		}

		public override string ToString()
		{
			return Value;
		}

		public bool Equals(Hash other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Hash) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		private static string GetNumber(int length)
		{
			if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

			var result = string.Empty;
			for (int i = 0; i < length; i++)
			{
				var minValue = i == 0 ? 1 : 0;
				var maxValue = 10;
				result += RandomShell.RandomService.NextInteger(minValue, maxValue);
			}

			return result;
		}

		private static string GetGuid(int partsCount)
		{
			if (partsCount <= 0) throw new ArgumentOutOfRangeException(nameof(partsCount));

			var guid = RandomShell.RandomService.NewGuid();
			if (partsCount >= 5)
				return guid.ToString("N");

			var parts = guid.ToString("D")
				.Split('-')
				.Take(partsCount);

			return string.Concat(parts);
		}
	}
}
