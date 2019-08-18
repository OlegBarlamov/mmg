using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.Services;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	[UsedImplicitly]
	internal class DefaultModelsProvider : IModelsProvider
	{
		[NotNull] private IControllersProvider ControllersProvider { get; }
		[NotNull] private IViewsProvider ViewsProvider { get; }
		[NotNull] private IAppDomainService AppDomainService { get; }

		public DefaultModelsProvider(IAppDomainService appDomainService, [NotNull] IViewsProvider viewsProvider,
			[NotNull] IControllersProvider controllersProvider)
		{
			ControllersProvider = controllersProvider ?? throw new ArgumentNullException(nameof(controllersProvider));
			ViewsProvider = viewsProvider ?? throw new ArgumentNullException(nameof(viewsProvider));
			AppDomainService = appDomainService ?? throw new ArgumentNullException(nameof(appDomainService));
		}

		public IEnumerable<Type> GetRegisteredModels()
		{
			var registeredViews = ViewsProvider.GetRegisteredViews();
			var registeredControllers = ControllersProvider.GetRegisteredControllers();

			var possibleModelNamesFromViews = ExtractPossibleModelNamesFromTypes(registeredViews);
			var possibleModelNamesFromControllers = ExtractPossibleModelNamesFromTypes(registeredControllers);
			var possibleModelNames = possibleModelNamesFromViews.Concat(possibleModelNamesFromControllers).ToArray();

			var allTypes = AppDomainService.GetAllTypes();
			return FilterTypesByPossibleModelNames(allTypes, possibleModelNames).Distinct();
		}

		private static IEnumerable<Type> FilterTypesByPossibleModelNames(IEnumerable<Type> allTypes,
			IReadOnlyList<string> possibleModelNames)
		{
			foreach (var type in allTypes)
			{
				var typeName = type.Name;
				if (possibleModelNames.Any(possibleName => IsCorretModelTypeName(typeName, possibleName)))
					yield return type;
			}
		}

		private static bool IsCorretModelTypeName(string candidateModelTypeName, string pattern)
		{
			return pattern.Equals(candidateModelTypeName, StringComparison.InvariantCultureIgnoreCase);
		}

		private static IEnumerable<string> ExtractPossibleModelNamesFromTypes(IEnumerable<Type> types)
		{
			var viewMarker = nameof(View);
			var controllerMarker = nameof(Controller);

			foreach (var type in types)
			{
				var name = type.Name;
				if (name.EndsWith(viewMarker, StringComparison.InvariantCultureIgnoreCase))
					yield return name.TrimEnd(viewMarker, StringComparison.InvariantCultureIgnoreCase);

				if (name.EndsWith(controllerMarker, StringComparison.InvariantCultureIgnoreCase))
					yield return name.TrimEnd(controllerMarker, StringComparison.InvariantCultureIgnoreCase);
			}
		}
	}
}
