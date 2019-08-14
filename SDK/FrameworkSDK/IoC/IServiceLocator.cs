using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceLocator : IDisposable
    {
	    [NotNull] object Resolve([NotNull] Type type);

        object ResolveWithParameter([NotNull] Type type, object parameter);

	    [NotNull, ItemNotNull] IReadOnlyList<object> ResolveMultiple([NotNull] Type type);

	    bool IsServiceRegistered([NotNull] Type type);
	}
}
