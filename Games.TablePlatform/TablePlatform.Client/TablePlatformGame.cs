using System;
using Console.Core;
using Console.FrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TablePlatform.Client
{
    [UsedImplicitly]
    internal class TablePlatformGame : GameApp
    {
        protected override Scene CurrentScene => _currentScene;
        
        private IConsoleController ConsoleController { get; }
        private IResourcesService ResourcesService { get; }
        private DefaultConsoleManipulator ConsoleManipulator { get; }

        private Scene _currentScene;
        
        private readonly GamePackage _package = new GamePackage();

        public TablePlatformGame(
            [NotNull] IConsoleController consoleController,
            [NotNull] IResourcesService resourcesService,
            [NotNull] DefaultConsoleManipulator consoleManipulator)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            ResourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
            ConsoleManipulator = consoleManipulator ?? throw new ArgumentNullException(nameof(consoleManipulator));

            _package.Loaded += PackageOnLoaded;
            ResourcesService.LoadPackage(_package);
        }

        private void PackageOnLoaded()
        {
            _currentScene = new GameScene(_package);
        }

        protected override void Dispose()
        {
            base.Dispose();
            
            _package.Dispose();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ConsoleController.Show();
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
        }

        protected override void OnContentUnloading()
        {
            base.OnContentUnloading();
            
            ResourcesService.UnloadPackage(_package);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            ConsoleManipulator.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
}