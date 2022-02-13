using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.DependencyInjection.Default.Misc;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default
{
	internal class DefaultDependencyResolver : IDependencyResolver
	{
		public object[] ResolveDependencies(IServiceLocator serviceLocator, ConstructorInfo constructor, object[] parameters)
	    {
		    if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
		    if (constructor == null) throw new ArgumentNullException(nameof(constructor));
	        if (parameters == null) parameters = Array.Empty<object>();

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

                return serviceLocator.Resolve(type);;
            }).ToArray();
	    }
	}
}
