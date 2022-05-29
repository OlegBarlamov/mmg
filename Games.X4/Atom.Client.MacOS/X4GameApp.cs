using System;
using Console.Core.Models;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.MacOS
{
    public class X4GameApp : GameApp
    {
        protected override SceneBase CurrentScene => _currentScene;

        private SceneBase _currentScene;
        
        private DefaultConsoleManipulator DefaultConsoleManipulator { get; }
        public IScenesContainer ScenesContainer { get; }
        public MainSceneDataModel MainSceneDataModel { get; }
        public CommandExecutorMediator CommandExecutorMediator { get; }
        public IAppTerminator AppTerminator { get; }

        private readonly IScenesResolver _scenesResolver;

        public X4GameApp([NotNull] DefaultConsoleManipulator defaultConsoleManipulator, [NotNull] IScenesContainer scenesContainer,
            [NotNull] MainSceneDataModel mainSceneDataModel, [NotNull] CommandExecutorMediator commandExecutorMediator,
            [NotNull] IAppTerminator appTerminator)
        {
            DefaultConsoleManipulator = defaultConsoleManipulator ?? throw new ArgumentNullException(nameof(defaultConsoleManipulator));
            ScenesContainer = scenesContainer ?? throw new ArgumentNullException(nameof(scenesContainer));
            MainSceneDataModel = mainSceneDataModel ?? throw new ArgumentNullException(nameof(mainSceneDataModel));
            CommandExecutorMediator = commandExecutorMediator ?? throw new ArgumentNullException(nameof(commandExecutorMediator));
            AppTerminator = appTerminator ?? throw new ArgumentNullException(nameof(appTerminator));

            ScenesContainer.RegisterScene<MainSceneDataModel, MainScene>();

            _scenesResolver = ScenesContainer.CreateResolver();
            
            CommandExecutorMediator.Command += CommandExecutorMediatorOnCommand;
            CommandExecutorMediator.AddCommand(new ConsoleCommand("exit", "Terminate app", "Close the application"));
        }

        private void CommandExecutorMediatorOnCommand(string command)
        {
            if (command == "exit")
            {
                AppTerminator.Terminate();
            }
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();

            _currentScene = _scenesResolver.ResolveScene(MainSceneDataModel);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            DefaultConsoleManipulator.Update(gameTime);
        }
    }
}