using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Common
{
	public static class NamesGenerator
	{
		public static string Hash([NotNull] Hash hash, [NotNull] string prefix = "", [NotNull] string separator = "_")
		{
			if (hash == null) throw new ArgumentNullException(nameof(hash));
			if (prefix == null) throw new ArgumentNullException(nameof(prefix));
			if (separator == null) throw new ArgumentNullException(nameof(separator));

			if (string.IsNullOrEmpty(prefix))
				return hash.Value;

			return $"{prefix}{separator}{hash}";
		}

		public static string Hash(HashType hashType, [NotNull] string prefix = "", [NotNull] string separator = "_")
		{
			return Hash(Common.Hash.Generate(hashType), prefix, separator);
		}
	}
}
