using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

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
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.NoPublicConstructorsException);

	        var correctConstructor = FilterConstructorsWithParameters(constructors, parameterType).FirstOrDefault();
            if (correctConstructor == null)
	            throw new FrameworkIocException(string.Format(Strings.Exceptions.Ioc.NoSuitablecConstructorsException, type));

	        return correctConstructor;
        }

        private IEnumerable<ConstructorInfo> FilterConstructors(IEnumerable<ConstructorInfo> constructors)
	    {
	        return constructors.OrderBy(info => info.GetParameters().Length)
	            .Where(IsCorrectConstructor);
	    }

	    private IEnumerable<ConstructorInfo> FilterConstructorsWithParameters(IEnumerable<ConstructorInfo> constructors, Type parameterType)
	    {
	        return constructors.OrderBy(info => info.GetParameters().Length)
	            .Where(info => IsCorrectConstructorWithParameter(info, parameterType));
	    }

        private bool IsCorrectConstructor([NotNull] ConstructorInfo constructorInfo)
		{
			if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));

			var parameters = constructorInfo.GetParameters().Select(info => info.ParameterType);
			return parameters.All(type => ServiceLocator.IsServiceRegistered(type));
		}

	    private bool IsCorrectConstructorWithParameter([NotNull] ConstructorInfo constructorInfo, [NotNull] IReadOnlyCollection<Type> parametersTypes)
	    {
	        if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
	        if (parametersTypes == null) throw new ArgumentNullException(nameof(parametersTypes));

	        var availableParameters = new List<Type>(parametersTypes);

	        var parameters = constructorInfo.GetParameters().Select(info => info.ParameterType);
	        return parameters.All(type => type.IsAssignableFrom(parameterType) || ServiceLocator.IsServiceRegistered(type));
	    }

	    private static ConstructorInfo[] GetConstructors(Type targetType)
	    {
	        return targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
	    }
    }
}
