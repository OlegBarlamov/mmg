using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceLocator : IDisposable
    {
	    [NotNull] T Resolve<T>();

	    [NotNull] object Resolve([NotNull] Type type);

	    [NotNull, ItemNotNull] IReadOnlyList<T> ResolveMultiple<T>();

	    [NotNull, ItemNotNull] IReadOnlyList<object> ResolveMultiple(Type type);

		bool IsServiceRegistered<T>();

	    bool IsServiceRegistered([NotNull] Type type);
	}
}
