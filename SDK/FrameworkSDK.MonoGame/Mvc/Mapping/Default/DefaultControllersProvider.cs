using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using NetExtensions;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
	[UsedImplicitly]
	internal class DefaultControllersProvider : IControllersProvider
	{
		private IAppDomainService AppDomainService { get; }

		public DefaultControllersProvider([NotNull] IAppDomainService appDomainService)
		{
			AppDomainService = appDomainService ?? throw new ArgumentNullException(nameof(appDomainService));
		}

		public IEnumerable<Type> GetRegisteredControllers()
		{
			var domainTypes = AppDomainService.GetAllTypes();
			return ExtractAvailableControllerTypes(domainTypes);
		}

		private static IEnumerable<Type> ExtractAvailableControllerTypes(IEnumerable<Type> domainTypes)
		{
			return domainTypes.Where(type =>
				type.IsClass &&
				!type.IsAbstract &&
				!type.IsInterface &&
				type.IsPublic &&
                !type.IsNested &&
				type.IsSubClassOf(typeof(Controller)));
		}
	}
}
