using System;
using Console.Core;
using FrameworkSDK.Constructing;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Constructing;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    internal class GameWithConsoleFactory : GameFactoryWrapper, IGameWithConsoleFactory
    {
        [NotNull] public IGameFactoryWithExternalComponents GameFactoryWithExternalComponents { get; }

        public GameWithConsoleFactory([NotNull] IGameFactoryWithExternalComponents gameFactoryWithExternalComponents)
            : base(gameFactoryWithExternalComponents)
        {
            GameFactoryWithExternalComponents = gameFactoryWithExternalComponents;
        }
        
        public IGameWithConsoleFactory UseConsoleMessagesProvider([NotNull] IConsoleMessagesProvider consoleMessagesProvider)
        {
            if (consoleMessagesProvider == null) throw new ArgumentNullException(nameof(consoleMessagesProvider));
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
                {
                    registrator.RegisterInstance(consoleMessagesProvider);
                }));
            return this;
        }

        public IGameWithConsoleFactory UseConsoleMessagesProvider<TConsoleMessageProvider>() where TConsoleMessageProvider : class, IConsoleMessagesProvider
        {
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterType<IConsoleMessagesProvider, TConsoleMessageProvider>();
            }));
            return this;
        }

        public IGameWithConsoleFactory UseConsoleCommandExecutor([NotNull] IConsoleCommandExecutor consoleCommandExecutor)
        {
            if (consoleCommandExecutor == null) throw new ArgumentNullException(nameof(consoleCommandExecutor));
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
                {
                    registrator.RegisterInstance(consoleCommandExecutor);
                }));
            return this;
        }

        public IGameWithConsoleFactory UseConsoleCommandExecutor<TConsoleCommandExecutor>() where TConsoleCommandExecutor : class, IConsoleCommandExecutor
        {
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterType<TConsoleCommandExecutor, TConsoleCommandExecutor>();
            }));
            return this;
        }
        
        public IGameWithConsoleFactory UseConsoleResourcePackage([NotNull] IConsoleResourcePackage consoleResourcePackage)
        {
            if (consoleResourcePackage == null) throw new ArgumentNullException(nameof(consoleResourcePackage));
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
                {
                    registrator.RegisterInstance(consoleResourcePackage);
                }));
            return this;
        }

        public IGameWithConsoleFactory UseConsoleResourcePackage<TConsoleResourcePackage>() where TConsoleResourcePackage : class, IConsoleResourcePackage
        {
            GameFactoryWithExternalComponents.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterType<TConsoleResourcePackage, TConsoleResourcePackage>();
            }));
            return this;
        }
    }
}