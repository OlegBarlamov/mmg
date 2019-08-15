using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	[UsedImplicitly]
	internal class DefaultViewsProvider : IViewsProvider
	{
		private IAppDomainService AppDomainService { get; }

		public DefaultViewsProvider([NotNull] IAppDomainService appDomainService)
		{
			AppDomainService = appDomainService ?? throw new ArgumentNullException(nameof(appDomainService));
		}

		public IEnumerable<Type> GetRegisteredViews()
		{
			var domainTypes = AppDomainService.GetAllTypes();
			return ExtractAvailableViewTypes(domainTypes);
		}

		private static IEnumerable<Type> ExtractAvailableViewTypes(IEnumerable<Type> domainTypes)
		{
			return domainTypes.Where(type =>
				type.IsClass &&
				!type.IsAbstract &&
				!type.IsInterface &&
				type.IsPublic &&
				type.IsSubClassOf(typeof(View)));
		}
	}
}
