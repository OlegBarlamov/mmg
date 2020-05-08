using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.Pipelines;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    public static class GameWithComponentsConfiguratorExtensions
    {
        public static IGameWithComponentsConfigurator<TGame> UseComponents<TGame>([NotNull] this IGameConfigurator<TGame> gameConfigurator) where TGame : GameApp
        {
            if (gameConfigurator == null) throw new ArgumentNullException(nameof(gameConfigurator));
            
            var newConfigurator = new GameWithComponentsConfigurator<TGame>(gameConfigurator);
            var service = newConfigurator.GameComponentsService;
            
            gameConfigurator.ConfigurationPipeline[GameDefaultConfigurationSteps.Registration]
                .AddAction(new SimplePipelineAction(GameDefaultConfigurationSteps.RegistrationActions.GameComponents, true,
                context =>
                {
                    var registrator = newConfigurator.GetObjectFromContext<IServiceRegistrator>(context, GameDefaultConfigurationSteps.ContextKeys.Container);
                    service.RegisterComponents(registrator);
                    registrator.RegisterInstance<IExternalGameComponentsService>(service);
                }));
            
            gameConfigurator.ConfigurationPipeline[GameDefaultConfigurationSteps.Constructing]
                .AddAction(new SimplePipelineAction(GameDefaultConfigurationSteps.ConstructingActions.GameComponents,
                    context =>
                    {
                        var locator = newConfigurator.GetObjectFromContext<IServiceLocator>(context, GameDefaultConfigurationSteps.ContextKeys.Locator);
                        service.ResolveComponents(locator);
                    }));

            return newConfigurator;
        }

        public static IGameWithComponentsConfigurator<TGame> AddComponent<TGame>(
            [NotNull] this IGameWithComponentsConfigurator<TGame> gameConfigurator, [NotNull] IExternalGameComponent component) where TGame : GameApp
        {
            if (gameConfigurator == null) throw new ArgumentNullException(nameof(gameConfigurator));
            if (component == null) throw new ArgumentNullException(nameof(component));
            
            var configurator = (GameWithComponentsConfigurator<TGame>)gameConfigurator;
            var service = configurator.GameComponentsService;
            service.AddComponent(component);
            
            return gameConfigurator;
        }
        
        public static IGameWithComponentsConfigurator<TGame> AddComponent<TGame, TComponent>(
            [NotNull] this IGameWithComponentsConfigurator<TGame> gameConfigurator) 
            where TGame : GameApp
            where TComponent : IExternalGameComponent
        {
            if (gameConfigurator == null) throw new ArgumentNullException(nameof(gameConfigurator));
            return gameConfigurator.AddComponent(typeof(TComponent));
        }
        
        public static IGameWithComponentsConfigurator<TGame> AddComponent<TGame>(
            [NotNull] this IGameWithComponentsConfigurator<TGame> gameConfigurator, [NotNull] Type componentType) 
            where TGame : GameApp 
        {
            if (gameConfigurator == null) throw new ArgumentNullException(nameof(gameConfigurator));
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            var configurator = (GameWithComponentsConfigurator<TGame>)gameConfigurator;
            var service = configurator.GameComponentsService;
            service.RegisterComponent(componentType);

            return gameConfigurator;
        }
    }
}