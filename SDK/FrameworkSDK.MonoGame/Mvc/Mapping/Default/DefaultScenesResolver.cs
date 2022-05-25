using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
    internal class DefaultScenesResolver : IScenesResolver
    {
        private IServiceLocator ServiceLocator { get; }

        private readonly ModuleLogger _logger;

        public DefaultScenesResolver([NotNull] IServiceLocator serviceLocator, [NotNull] IFrameworkLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

            _logger = new ModuleLogger(logger, LogCategories.Mvc);
        }

        public bool IsModelRegistered(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var type = model.GetType();
            return ServiceLocator.IsServiceRegistered(type);
        }

        public SceneBase ResolveScene(object model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                var type = model.GetType();
                var scene = (IScene) ServiceLocator.Resolve(type, new[] {model});
                if (scene.DataModel == null)
                    scene.DataModel = model;

                _logger.Debug(Strings.Info.Mapping.SceneResolvedByModel, scene, model);
                return (SceneBase) scene;
            }
            catch (Exception e)
            {
                throw new MappingException(Strings.Exceptions.Mapping.ErrorWhileResolvingScene, e, model);
            }
        }

        public void Dispose()
        {
            _logger.Dispose();
        }
    }
}
