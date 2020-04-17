using System;
using Console.Core;
using Console.InGame;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameExtensions;

namespace TablePlatform.Client
{
    internal class TablePlatformGame : GameApplication
    {
        public override Scene CurrentScene { get; } = new EmptyScene();
        
        public IConsoleMessagesProvider ConsoleMessagesProvider { get; }
        public IConsoleCommandExecutor ConsoleCommandExecutor { get; }

        private InGameConsoleController _inGameConsoleController;

        private bool _initialized;

        public TablePlatformGame(IConsoleMessagesProvider consoleMessagesProvider, IConsoleCommandExecutor consoleCommandExecutor)
        {
            ConsoleMessagesProvider = consoleMessagesProvider;
            ConsoleCommandExecutor = consoleCommandExecutor;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            Content.RootDirectory = "Resources";
        }

        private SpriteBatch _spriteBatch;
        
        protected override void Update(GameTime gameTime)
        {
            if (!_initialized)
            {
                _initialized = true;
                var background = new Color(31,32,36);
                var headerAmbient = new Color(26, 54, 84);
                _inGameConsoleController = new InGameConsoleController(ConsoleMessagesProvider, ConsoleCommandExecutor, new InGameConsoleConfig
                {
                    DefaultWidth = 800,
                    HeaderBackground = Game.GraphicsDeviceManager.GraphicsDevice.GetTextureGradientColor(headerAmbient, background, 30, 20, 90, 0.8f),
                    Background = Game.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(background),
                    CommandLineCorner = Game.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(Color.White),
                    SuggestSelection = Game.GraphicsDeviceManager.GraphicsDevice.GetTextureDiffuseColor(Color.Orange),
                    ConsoleFont = Content.Load<SpriteFont>("TextFont")
                }, Game.GraphicsDeviceManager.GraphicsDevice);
                
                _spriteBatch = new SpriteBatch(Game.GraphicsDeviceManager.GraphicsDevice);
                
                _inGameConsoleController.LoadContent();
                _inGameConsoleController.Show();
            }
            
            base.Update(gameTime);
            
            _inGameConsoleController.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            _inGameConsoleController.Draw(gameTime, _spriteBatch);
        }
    }
}