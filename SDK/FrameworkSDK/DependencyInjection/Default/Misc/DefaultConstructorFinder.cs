using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.DependencyInjection.Default.Misc;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace FrameworkSDK.DependencyInjection.Default
{
	internal class DefaultConstructorFinder : IConstructorFinder
	{
		public ConstructorInfo GetConstructor(IServiceLocator serviceLocator, Type type, Type[] parametersTypes = null)
	    {
		    if (serviceLocator == null) throw new ArgumentNullException(nameof(serviceLocator));
		    if (type == null) throw new ArgumentNullException(nameof(type));
	        if (parametersTypes == null) parametersTypes = Array.Empty<Type>();

	        var constructors = GetConstructors(type);
            if (constructors.Length < 1)
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.NoPublicConstructorsException, type.Name);

	        var correctConstructor = FilterConstructorsWithParameters(serviceLocator, constructors, parametersTypes, out var lastUnresolvedType).FirstOrDefault();
		    if (correctConstructor == null)
		    {
		        if (parametersTypes.Any())
					throw new FrameworkIocException(Strings.Exceptions.Ioc.NoSuitablecConstructorsExceptionWithParameters, type, parametersTypes.ArrayToString(), lastUnresolvedType);

		        throw new FrameworkIocException(Strings.Exceptions.Ioc.NoSuitablecConstructorsException, type, lastUnresolvedType);
		    }
	            
	        return correctConstructor;
        }

	    private IEnumerable<ConstructorInfo> FilterConstructorsWithParameters(IServiceLocator serviceLocator, IEnumerable<ConstructorInfo> constructors, IReadOnlyCollection<Type> parameterTypes, out Type lastUnresolvedType)
	    {
		    Type unresolvedType = null;
		    var result = constructors.OrderBy(info => info.GetParameters().Length)
	            .Where(info => IsCorrectConstructorWithParameters(serviceLocator, info, parameterTypes, out unresolvedType)).ToArray();
		    lastUnresolvedType = unresolvedType;
		    return result;
	    }

	    private bool IsCorrectConstructorWithParameters(IServiceLocator serviceLocator, [NotNull] ConstructorInfo constructorInfo, [NotNull] IReadOnlyCollection<Type> parametersTypes, out Type unresolvedType)
	    {
		    var availableParameters = new List<Type>(parametersTypes);
	        var parameters = constructorInfo.GetParameters().Select(info => info.ParameterType);

		    foreach (var parameter in parameters)
		    {
			    var correctParameter = availableParameters.FirstOrDefault(type => parameter.IsAssignableFrom(type));
			    if (correctParameter != null)
			    {
				    availableParameters.Remove(correctParameter);
					continue;
			    }

			    if (!serviceLocator.IsServiceRegistered(parameter))
			    {
				    unresolvedType = parameter;
				    return false;
			    }
		    }

		    unresolvedType = null;
		    return true;
	    }

	    private static ConstructorInfo[] GetConstructors(Type targetType)
	    {
	        return targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
		        //TODO move to the Construtoris prioritizer
		        .Where(info => info.GetCustomAttribute<ObsoleteAttribute>() == null).ToArray();
	    }
    }
}
