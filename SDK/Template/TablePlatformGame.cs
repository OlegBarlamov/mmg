using System;
using Console.Core;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
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

        private Scene _currentScene;

        public TablePlatformGame([NotNull] IConsoleController consoleController)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
        }

        protected override void Dispose()
        {
            base.Dispose();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _currentScene = new EmptyScene();
            
            ConsoleController.Show();
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
        }

        protected override void OnContentUnloading()
        {
            base.OnContentUnloading();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
}