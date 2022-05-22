using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
    public class DefaultMvcMappingProvider : IMvcMappingProvider, IDisposable
    {
        private IViewsProvider ViewsProvider { get; }
        private IControllersProvider ControllersProvider { get; }
        private ModuleLogger Logger { get; }
        private readonly List<MvcTypesDeclaration> _mapping = new List<MvcTypesDeclaration>();

        public DefaultMvcMappingProvider([NotNull] IViewsProvider viewsProvider, [NotNull] IControllersProvider controllersProvider, IFrameworkLogger logger)
        {
            ViewsProvider = viewsProvider ?? throw new ArgumentNullException(nameof(viewsProvider));
            ControllersProvider = controllersProvider ?? throw new ArgumentNullException(nameof(controllersProvider));
            Logger = new ModuleLogger(logger, LogCategories.Mvc);
        }
        
        public void FetchMapping()
        {
            Logger.Debug("Fetching by default MVC provider");
            
            var views = ViewsProvider.GetRegisteredViews();
            var controllers = ControllersProvider.GetRegisteredControllers();

            var mappingsFromViews = FindMappingsFromViews(views).ToArray();
            var notMappedYetControllers = controllers
                .Where(controller => mappingsFromViews.All(x => x.Controller != controller))
                .ToArray();
            var mappingFromControllers = FindMappingsFromControllers(notMappedYetControllers);

            _mapping.AddRange(mappingsFromViews);
            _mapping.AddRange(mappingFromControllers);
            
            Logger.Debug("Fetched groups: ");
            foreach (var group in _mapping)
            {
                Logger.Debug(group.ToString());
            }
            Logger.Debug($"Total MVC groups: {_mapping.Count}");
        }

        public IReadOnlyList<MvcTypesDeclaration> GetMapping()
        {
            return _mapping;
        }

        public void Dispose()
        {
            _mapping.Clear();
        }

        private IEnumerable<MvcTypesDeclaration> FindMappingsFromViews(IEnumerable<Type> viewsTypes)
        {
            foreach (var viewType in viewsTypes)
            {
                // View<TData, TController>
                var viewTypeArguments = GetBaseTypeGenericTypeArguments(viewType, typeof(View<,>));
                if (viewTypeArguments.Length > 0)
                {
                    var modelType = viewTypeArguments[0];
                    var controllerType = viewTypeArguments[1];
                    yield return new MvcTypesDeclaration(modelType, viewType, controllerType);
                }
            }
        }

        private IEnumerable<MvcTypesDeclaration> FindMappingsFromControllers(IEnumerable<Type> controllersTypes)
        {
            foreach (var controllerType in controllersTypes)
            {
                // Controller<TData>
                var viewTypeArguments = GetBaseTypeGenericTypeArguments(controllerType, typeof(Controller<>));
                if (viewTypeArguments.Length > 0)
                {
                    var modelType = viewTypeArguments[0];
                    yield return new MvcTypesDeclaration(modelType, null, controllerType);
                }
            }
        }
        
        private Type[] GetBaseTypeGenericTypeArguments(Type targetType, Type searchedGenericTypeDefinition)
        {
            if (!searchedGenericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException($"{nameof(searchedGenericTypeDefinition)} parameter must be genericTypeDefinition");
            
            while (targetType.BaseType != null)
            {
                targetType = targetType.BaseType;
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == searchedGenericTypeDefinition)
                    return targetType.GetGenericArguments();
            }

            return Array.Empty<Type>();
        }
    }
}