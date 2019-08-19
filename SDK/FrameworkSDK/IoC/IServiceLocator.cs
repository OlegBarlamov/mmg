using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceLocator
    {
	    [NotNull] object Resolve([NotNull] Type type);

        [NotNull] object ResolveWithParameters([NotNull] Type type, [NotNull, ItemNotNull] object[] parameters);

	    [NotNull, ItemNotNull] IReadOnlyList<object> ResolveMultiple([NotNull] Type type);

	    bool IsServiceRegistered([NotNull] Type type);
	}
}
