using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Localization;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = FrameworkSDK.MonoGame.Basic.IDrawable;
using IUpdateable = FrameworkSDK.MonoGame.Basic.IUpdateable;

namespace FrameworkSDK.MonoGame
{
    public abstract class GameApp : IGameHost
    {
        public event EventHandler Disposed;

        protected abstract Scene CurrentScene { get; }
	    
        private IScenesController ScenesController { get; } 
            = AppContext.ServiceLocator.Resolve<IScenesController>();
        private IGameHeartServices GameHeartServices { get; } =
            AppContext.ServiceLocator.Resolve<IGameHeartServices>();

        private IInputManager InputManager { get; } =
            AppContext.ServiceLocator.Resolve<IInputManager>();

        private static bool _oneInstanceCreated;
        private readonly IReadOnlyCollection<IExternalGameComponent> _externalGameComponents;
        
        protected GameApp()
        {
            if (_oneInstanceCreated) throw new FrameworkMonoGameException(Strings.Exceptions.Constructing.MultipleGameInstances, GetType().Name);
            _oneInstanceCreated = true;

            _externalGameComponents = AppContext.ServiceLocator.Resolve<IExternalGameComponentsService>().GetComponents();
        }

        protected virtual void OnInitialized()
        {
            
        }

        protected virtual void OnContentLoaded()
        {
            
        }
        
        protected virtual void Update(GameTime gameTime)
        {

        }

        protected virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

        protected virtual void OnContentUnloading()
        {
            
        }

        protected virtual void Dispose()
        {
            
        }
        
        void IDisposable.Dispose()
        {
            foreach (var component in _externalGameComponents)
            {
                component.Dispose();
            }

            Dispose();
            
            ScenesController.Dispose();
            
            Disposed?.Invoke(this, EventArgs.Empty);
            Disposed = null;
        }
        
        void IApplication.Run()
        {
        }

        void IGameHost.OnInitialize()
        {
            foreach (var component in _externalGameComponents)
            {
                component.Initialize();
            }
            
            OnInitialized();
        }

        void IGameHost.OnLoadContent()
        {
            foreach (var component in _externalGameComponents)
            {
                component.LoadContent();
            }
            
            OnContentLoaded();
        }

        void IUpdateable.Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
            
            Update(gameTime);
            
            foreach (var component in _externalGameComponents)
            {
                component.Update(gameTime);
            }

            if (ScenesController.CanSceneChange)
                ScenesController.CurrentScene = CurrentScene;

            ScenesController.Update(gameTime);
        }

        void IDrawable.Draw(GameTime gameTime)
        {
            ScenesController.Draw(gameTime);
            
            foreach (var component in _externalGameComponents)
            {
                component.Draw(gameTime);
            }
            
            Draw(gameTime, GameHeartServices.SpriteBatch);
        }

        void IGameHost.OnUnloadContent()
        {
            foreach (var component in _externalGameComponents)
            {
                component.UnloadContent();
            }
            
            OnContentUnloading();
        }
    }
}
