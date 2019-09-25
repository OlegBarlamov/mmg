using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.IoC.Default
{
	internal class ConstructorFinder
	{
		private IServiceLocator ServiceLocator { get; }

		public ConstructorFinder([NotNull] IServiceLocator serviceLocator)
		{
			ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
		}

	    public ConstructorInfo GetConstructor(Type type)
	    {
	        return this.GetConstructorWithParameters(type);
	    }

        public ConstructorInfo GetConstructorWithParameters([NotNull] Type type, [NotNull] Type[] parametersTypes)
	    {
	        if (type == null) throw new ArgumentNullException(nameof(type));
	        if (parametersTypes == null) throw new ArgumentNullException(nameof(parametersTypes));

	        var constructors = GetConstructors(type);
            if (constructors.Length < 1)
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.NoPublicConstructorsException, type.Name);

	        var correctConstructor = FilterConstructorsWithParameters(constructors, parametersTypes).FirstOrDefault();
		    if (correctConstructor == null)
		    {
		        if (parametersTypes.Any())
					throw new FrameworkIocException(Strings.Exceptions.Ioc.NoSuitablecConstructorsExceptionWithParameters, type, parametersTypes.ArrayToString());

		        throw new FrameworkIocException(Strings.Exceptions.Ioc.NoSuitablecConstructorsException, type);
		    }
	            
	        return correctConstructor;
        }

	    private IEnumerable<ConstructorInfo> FilterConstructorsWithParameters(IEnumerable<ConstructorInfo> constructors, IReadOnlyCollection<Type> parameterTypes)
	    {
	        return constructors.OrderBy(info => info.GetParameters().Length)
	            .Where(info => IsCorrectConstructorWithParameters(info, parameterTypes));
	    }

	    private bool IsCorrectConstructorWithParameters([NotNull] ConstructorInfo constructorInfo, [NotNull] IReadOnlyCollection<Type> parametersTypes)
	    {
	        if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
	        if (parametersTypes == null) throw new ArgumentNullException(nameof(parametersTypes));

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

			    if (!ServiceLocator.IsServiceRegistered(parameter))
				    return false;
		    }

		    return true;
	    }

	    private static ConstructorInfo[] GetConstructors(Type targetType)
	    {
	        return targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
	    }
    }
}
