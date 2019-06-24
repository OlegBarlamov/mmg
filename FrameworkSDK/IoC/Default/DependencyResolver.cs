using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC.Default
{
	internal class DependencyResolver
	{
		private IServiceLocator ServiceLocator { get; }

		public DependencyResolver([NotNull] IServiceLocator serviceLocator)
		{
			ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
		}

		public object[] ResolveDependencies(ConstructorInfo constructor)
		{
			if (constructor == null) throw new ArgumentNullException(nameof(constructor));

			var parameters = constructor.GetParameters().Select(info => info.ParameterType).ToArray();

			CheckAllRegistered(parameters);

			return parameters.Select(type => ServiceLocator.Resolve(type)).ToArray();
		}

		private void CheckAllRegistered(IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				if (!ServiceLocator.IsServiceRegistered(type))
					throw new FrameworkIocException(Strings.Exceptions.Ioc.DependencyNotResolvedException);
			}
		}
	}
}
