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

	    public ConstructorInfo GetConstructorWithParameter([NotNull] Type type, [NotNull] Type parameterType)
	    {
	        if (type == null) throw new ArgumentNullException(nameof(type));
	        if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

	        var constructors = GetConstructors(type);
            if (constructors.Length < 1)
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.NoPublicConstructorsException);

	        var correctConstructor = FilterConstructorsWithParameters(constructors, parameterType).FirstOrDefault();
            if (correctConstructor == null)
	            throw new FrameworkIocException(string.Format(Strings.Exceptions.Ioc.NoSuitablecConstructorsException, type));

	        return correctConstructor;
        }

	    public ConstructorInfo FindConstructorWithParameter([NotNull] Type type, [NotNull] Type parameterType)
	    {
	        if (type == null) throw new ArgumentNullException(nameof(type));
	        if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

	        var constructors = GetConstructors(type);
            return constructors.Length < 1 ? null : FilterConstructorsWithParameters(constructors, parameterType).FirstOrDefault();
	    }

        public ConstructorInfo GetConstructor(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			var constructors = GetConstructors(type);
            if (constructors.Length < 1)
				throw new FrameworkIocException(String.Format(Strings.Exceptions.Ioc.NoPublicConstructorsException, type));

			var correctConstructor = FilterConstructors(constructors).FirstOrDefault();
			if (correctConstructor == null)
				throw new FrameworkIocException(string.Format(Strings.Exceptions.Ioc.NoSuitablecConstructorsException, type));

			return correctConstructor;
		}

	    public ConstructorInfo FindConstructor([NotNull] Type type)
	    {
	        if (type == null) throw new ArgumentNullException(nameof(type));

	        var constructors = GetConstructors(type);
	        return constructors.Length < 1 ? null : FilterConstructors(constructors).FirstOrDefault();
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

	    private bool IsCorrectConstructorWithParameter([NotNull] ConstructorInfo constructorInfo, [NotNull] Type parameterType)
	    {
	        if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
	        if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

	        var parameters = constructorInfo.GetParameters().Select(info => info.ParameterType);
	        return parameters.All(type => type.IsAssignableFrom(parameterType) || ServiceLocator.IsServiceRegistered(type));
	    }

	    private static ConstructorInfo[] GetConstructors(Type targetType)
	    {
	        return targetType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
	    }
    }
}
