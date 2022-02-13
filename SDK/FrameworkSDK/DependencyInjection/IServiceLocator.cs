using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection
{
    public interface IServiceLocator : IServiceFactory
    {
	    [NotNull] object Resolve([NotNull] Type type, [ItemNotNull] object[] additionalParameters = null);

	    [NotNull, ItemNotNull] Array ResolveMultiple([NotNull] Type type);

	    bool IsServiceRegistered([NotNull] Type type);
	}
}
