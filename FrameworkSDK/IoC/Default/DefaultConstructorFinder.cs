using System;
using System.Linq;
using System.Reflection;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC.Default
{
	internal class DefaultConstructorFinder : IConstructorFinder
	{
		public IServiceLocator ServiceLocator { get; set; }

		public ConstructorInfo FindConstructor(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			var constructors = type.GetConstructors(BindingFlags.Public);
			if (constructors.Length < 1)
				throw new FrameworkIoCException(Strings.Exceptions.Ioc.NoPublicConstructorsException);

			var sortedConstructors = constructors.OrderBy(info => info.GetParameters().Length);
			var correctConstructor = sortedConstructors.FirstOrDefault(IsCorrectConstructor);
			if (correctConstructor == null)
				throw new FrameworkIoCException(Strings.Exceptions.Ioc.NoSuitablecConstructorsException);

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
