using System;
using JetBrains.Annotations;

namespace FrameworkSDK.Configuration
{
	public static class NamesObjectsHeapExtensions
	{
		[CanBeNull]
		public static T GetObject<T>([NotNull] this NamedObjectsHeap namedObjectsHeap, [NotNull] string key)
		{
			if (namedObjectsHeap == null) throw new ArgumentNullException(nameof(namedObjectsHeap));
			if (key == null) throw new ArgumentNullException(nameof(key));

			var @object = namedObjectsHeap.GetObject(key);

			try
			{
				return (T)@object;
			}
			catch (Exception)
			{
				return default(T);
			}
		}
	}
}
