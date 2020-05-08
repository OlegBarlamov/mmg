using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Console.Core;
using Console.Core.Models;
using Console.InGame.Implementation;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Console.InGame
{
    public class InGameConsoleController : IConsoleController
    {
        public bool IsShowed => _isShowed;

        private IConsoleMessagesProvider ConsoleMessagesProvider { get; }
        private IConsoleCommandExecutor ConsoleCommandExecutor { get; }

        private bool _isShowed;
        private bool _isDrawEnabled;
        private bool _isDisposed;

        private readonly ConsoleModel _model;
        private readonly ConsoleView _view;
        private readonly RenderersProvider _renderersProvider = new RenderersProvider();

        public InGameConsoleController(
            [NotNull] IConsoleMessagesProvider consoleMessagesProvider,
            [NotNull] IConsoleCommandExecutor consoleCommandExecutor)
        {
            ConsoleMessagesProvider = consoleMessagesProvider ?? throw new ArgumentNullException(nameof(consoleMessagesProvider));
            ConsoleCommandExecutor = consoleCommandExecutor ?? throw new ArgumentNullException(nameof(consoleCommandExecutor));

            _model = new ConsoleModel(consoleCommandExecutor, _renderersProvider);
            _view = new ConsoleView(_model, _renderersProvider);
            
            _model.UserCommand += ModelOnUserCommand;
            _view.ShowAnimationFinished += ViewOnShowAnimationFinished;
            _view.HideAnimationFinished += ViewOnHideAnimationFinished;
        }

        public void Initialize([NotNull] InGameConsoleConfig config, [NotNull] GraphicsDevice graphicsDevice)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (graphicsDevice == null) throw new ArgumentNullException(nameof(graphicsDevice));
            
            CheckConfigValid(config);
            
            _model.Initialize(config);
            _view.Initialize(config, graphicsDevice);
            
            ConsoleMessagesProvider.NewMessages += ConsoleMessagesProviderOnNewMessages;
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController)); 
                
            _isDisposed = true;
            _isShowed = false;
            _isDrawEnabled = false;
            ConsoleMessagesProvider.NewMessages -= ConsoleMessagesProviderOnNewMessages;
            _model.IsAllowControl = false;
            _model.UserCommand -= ModelOnUserCommand;
            _view.ShowAnimationFinished -= ViewOnShowAnimationFinished;
            _view.HideAnimationFinished -= ViewOnHideAnimationFinished;
            _view.Dispose();
            _model.Dispose();
        }

        public void RegisterDataRenderer<TData>(IDataRenderer renderer)
        {
            RegisterDataRenderer(typeof(TData), renderer);
        }
        
        public void RegisterDataRenderer([NotNull] Type dataType, [NotNull] IDataRenderer renderer)
        {
            if (dataType == null) throw new ArgumentNullException(nameof(dataType));
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            
            if (_renderersProvider.IsRegistered(dataType))
                throw new ArgumentException($"{dataType} already registered");
            
            _renderersProvider.Register(dataType, renderer);
        }

        public void Show()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            
            _isShowed = true;
            _isDrawEnabled = true;
            _view.Show();
            _model.OnShow();
        }

        public void Hide()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            
            _isShowed = false;
            _model.IsAllowControl = false;
            _view.Hide();
        }

        public void ClearCurrent()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            
            _model.Clear();
        }

        public void ClearAll()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            
            _model.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            
            if (_model.IsAllowControl)
                _model.Update(gameTime);
            
            if (_isDrawEnabled)
                _view.Update(gameTime);
        }

        public void Draw(GameTime gameTime, [NotNull] SpriteBatch spriteBatch)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(InGameConsoleController));
            if (spriteBatch == null) throw new ArgumentNullException(nameof(spriteBatch));
            
            if (_isDrawEnabled)
                _view.Draw(gameTime, spriteBatch);
        }
        
        private void ViewOnShowAnimationFinished()
        {
            _model.IsAllowControl = true;
        }
        
        private void ViewOnHideAnimationFinished()
        {
            _isDrawEnabled = false;
        }
        
        private void ModelOnUserCommand(string command)
        {
            ConsoleCommandExecutor.ExecuteAsync(command);
        }
        
        private void ConsoleMessagesProviderOnNewMessages()
        {
            var messages = new List<IConsoleMessage>();
            while (!ConsoleMessagesProvider.IsQueueEmpty)
            {
                var message = ConsoleMessagesProvider.Pop();
                messages.Add(message);
            }
            
            _model.AddMessages(messages);
        }

        private void CheckConfigValid(InGameConsoleConfig config)
        {
            if (config.Background == null) throw new ArgumentNullException(nameof(config.Background));
            if (config.ConsoleFont == null) throw new ArgumentNullException(nameof(config.ConsoleFont));
            if (config.HeaderBackground == null) throw new ArgumentNullException(nameof(config.HeaderBackground));
            if (config.CommandLineCorner == null) throw new ArgumentNullException(nameof(config.CommandLineCorner));
            if (config.SuggestSelection == null) throw new ArgumentNullException(nameof(config.SuggestSelection));
        }
    }
}