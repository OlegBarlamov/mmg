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

        //TODO refactor
	    public object[] ResolveDependenciesWithParameter(ConstructorInfo constructor, [NotNull] object parameter)
	    {
	        if (constructor == null) throw new ArgumentNullException(nameof(constructor));
	        if (parameter == null) throw new ArgumentNullException(nameof(parameter));

	        var parameters = constructor.GetParameters().Select(info => info.ParameterType).ToArray();

	        CheckAllRegistered(parameters, parameter);

	        var existingParameters = new List<object> {parameter};
            return parameters.Select(type =>
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

	    private void CheckAllRegistered(IEnumerable<Type> types, [NotNull, ItemNotNull] params object[] exsisting)
	    {
	        if (exsisting == null) throw new ArgumentNullException(nameof(exsisting));
	        var existingParameters = new List<Type>(exsisting.Select(o => o.GetType()));
	        foreach (var type in types)
	        {
                //TODO тут в общем случае может разрешится в неверном порядке.
	            var existing = existingParameters.FirstOrDefault(existingType => type.IsAssignableFrom(existingType));
	            if (existing != null)
	            {
	                existingParameters.Remove(existing);
                    continue;
	            }

	            if (!ServiceLocator.IsServiceRegistered(type))
	                throw new FrameworkIocException(string.Format(Strings.Exceptions.Ioc.DependencyNotResolvedException, type));
	        }
	    }
    }
}
