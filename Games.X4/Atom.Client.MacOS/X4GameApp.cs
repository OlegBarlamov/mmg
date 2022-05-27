using System;
using Atom.Client.MacOS.Components;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources.Generation;
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

        private readonly IScenesResolver _scenesResolver;

        public X4GameApp([NotNull] DefaultConsoleManipulator defaultConsoleManipulator, [NotNull] IScenesContainer scenesContainer,
            [NotNull] MainSceneDataModel mainSceneDataModel)
        {
            DefaultConsoleManipulator = defaultConsoleManipulator ?? throw new ArgumentNullException(nameof(defaultConsoleManipulator));
            ScenesContainer = scenesContainer ?? throw new ArgumentNullException(nameof(scenesContainer));
            MainSceneDataModel = mainSceneDataModel ?? throw new ArgumentNullException(nameof(mainSceneDataModel));

            ScenesContainer.RegisterScene<MainSceneDataModel, MainScene>();

            _scenesResolver = ScenesContainer.CreateResolver();
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