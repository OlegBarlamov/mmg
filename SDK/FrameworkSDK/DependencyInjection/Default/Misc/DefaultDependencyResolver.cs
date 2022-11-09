using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.DependencyInjection.Default.Misc;

namespace FrameworkSDK.DependencyInjection.Default
{
	internal class DefaultDependencyResolver : IDependencyResolver
	{
		public object[] ResolveDependencies(IServiceLocator serviceLocator, Type[] dependencies, object[] parameters)
	    {
		    if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
		    if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
		    if (parameters == null) parameters = Array.Empty<object>();

	        var existingParameters = new List<object>(parameters);
            return dependencies.Select(type =>
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
