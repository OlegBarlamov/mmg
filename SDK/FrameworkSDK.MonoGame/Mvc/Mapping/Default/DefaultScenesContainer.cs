using System;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
    internal class DefaultScenesContainer : IScenesContainer
    {
        [NotNull] private IFrameworkLogger Logger { get; }
        [NotNull] private IFrameworkServiceContainer ServiceContainer { get; }

        private IFrameworkServiceContainer ScenesContainer => _scenesContainer ?? (_scenesContainer = ServiceContainer.CreateScoped("scenes"));

        private IFrameworkServiceContainer _scenesContainer;

        private readonly ModuleLogger _moduleLogger;

        public DefaultScenesContainer([NotNull] IFrameworkServiceContainer serviceContainer, [NotNull] IFrameworkLogger logger)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _moduleLogger = new ModuleLogger(Logger, FrameworkLogModule.Mvc);
        }

        public void RegisterScene(Type modelType, Type sceneType)
        {
            ScenesContainer.RegisterType(modelType, sceneType, ResolveType.InstancePerResolve);
            _moduleLogger.Debug(Strings.Info.Mapping.SceneRegisteredForModel, sceneType, modelType);
        }

        public void Dispose()
        {
            _moduleLogger.Dispose();
            ScenesContainer.Dispose();
        }

        public IScenesResolver CreateResolver()
        {
            var locator = ScenesContainer.BuildContainer();
            return new DefaultScenesResolver(locator, Logger);
        }
    }
}
