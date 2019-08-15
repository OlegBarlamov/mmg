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

	    public object[] ResolveDependenciesWithParameters(ConstructorInfo constructor, [NotNull, ItemNotNull] object[] parameters)
	    {
	        if (constructor == null) throw new ArgumentNullException(nameof(constructor));
	        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

	        var constructorParameters = constructor.GetParameters().Select(info => info.ParameterType).ToArray();

	        var existingParameters = new List<object>(parameters);
            return constructorParameters.Select(type =>
            {
                var fromParameters = existingParameters.FirstOrDefault(type.IsInstanceOfType);
                if (fromParameters != null)
                {
                    existingParameters.Remove(fromParameters);
                    return fromParameters;
                }

                return ServiceLocator.Resolve(type);
            }).ToArray();
	    }

		private void CheckAllRegistered([NotNull] IEnumerable<Type> types)
	    {
		    if (types == null) throw new ArgumentNullException(nameof(types));
	        foreach (var type in types)
	        {
	            if (!ServiceLocator.IsServiceRegistered(type))
	                throw new FrameworkIocException(string.Format(Strings.Exceptions.Ioc.DependencyNotResolvedException, type.Name));
	        }
	    }
    }
}
