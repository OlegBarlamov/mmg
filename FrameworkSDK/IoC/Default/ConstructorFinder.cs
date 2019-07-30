using System;
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

		public ConstructorInfo FindConstructor(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			var constructors = type.GetConstructors(BindingFlags.Public);
			if (constructors.Length < 1)
				throw new FrameworkIocException(Strings.Exceptions.Ioc.NoPublicConstructorsException);

			var sortedConstructors = constructors.OrderBy(info => info.GetParameters().Length);
			var correctConstructor = sortedConstructors.FirstOrDefault(IsCorrectConstructor);
			if (correctConstructor == null)
				throw new FrameworkIocException(Strings.Exceptions.Ioc.NoSuitablecConstructorsException);

			return correctConstructor;
		}

		private bool IsCorrectConstructor([NotNull] ConstructorInfo constructorInfo)
		{
			if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));

			var parameters = constructorInfo.GetParameters().Select(info => info.ParameterType);
			return parameters.All(type => ServiceLocator.IsServiceRegistered(type));
		}
	}
}
