using System;
using Console.Core;
using Console.Core.Implementations;
using Console.InGame;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    public static class GameWithComponentsExtensions
    {
        public static IGameWithComponentsConfigurator<TGame> AddConsole<TGame>(
            [NotNull] this IGameWithComponentsConfigurator<TGame> gameConfigurator,
            [NotNull] IConsoleMessagesProvider consoleMessagesProvider,
            [CanBeNull] IConsoleResourcePackage consoleResourcePackage = null)
            where TGame : GameApp
        {
            if (gameConfigurator == null) throw new ArgumentNullException(nameof(gameConfigurator));
            if (consoleMessagesProvider == null) throw new ArgumentNullException(nameof(consoleMessagesProvider));

            return gameConfigurator.AddConsole(consoleMessagesProvider, new EmptyConsoleCommandExecutor(),
                consoleResourcePackage);
        }

        public static IGameWithComponentsConfigurator<TGame> AddConsole<TGame>(
            [NotNull] this IGameWithComponentsConfigurator<TGame> gameConfigurator,
            [NotNull] IConsoleMessagesProvider consoleMessagesProvider,
            [NotNull] IConsoleCommandExecutor consoleCommandExecutor,
            [CanBeNull] IConsoleResourcePackage consoleResourcePackage = null)
            where TGame : GameApp
        {
            if (consoleResourcePackage == null) 
                consoleResourcePackage = new DefaultResourcePackage();

            gameConfigurator.RegisterServices((registrator) =>
            {
                registrator.RegisterType<IConsoleController, InGameConsoleController>();
                registrator.RegisterInstance<IConsoleMessagesProvider>(consoleMessagesProvider);
                registrator.RegisterInstance<IConsoleCommandExecutor>(consoleCommandExecutor);
                registrator.RegisterInstance<IConsoleResourcePackage>(consoleResourcePackage);
                registrator.RegisterType<DefaultConsoleManipulator, DefaultConsoleManipulator>();
            });
            
            return gameConfigurator.AddComponent<TGame, ConsoleGameComponent>();
        }
    }
}