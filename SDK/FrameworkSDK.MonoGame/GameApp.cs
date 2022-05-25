using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Localization;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = FrameworkSDK.MonoGame.Basic.IDrawable;

namespace FrameworkSDK.MonoGame
{
    public abstract class GameApp : IGameHost
    {
        public event EventHandler DisposedEvent;
        public bool IsDisposed { get; private set; }

        protected abstract SceneBase CurrentScene { get; }
	    
        private IScenesController ScenesController { get; } 
            = AppContext.ServiceLocator.Resolve<IScenesController>();
        private IGameHeartServices GameHeartServices { get; } =
            AppContext.ServiceLocator.Resolve<IGameHeartServices>();

        private IInputManager InputManager { get; } =
            AppContext.ServiceLocator.Resolve<IInputManager>();

        private IGraphicsPipelineFactoryService GraphicsPipelineFactoryService { get; } =
            AppContext.ServiceLocator.Resolve<IGraphicsPipelineFactoryService>();
        
        private static bool _oneInstanceCreated;
        private readonly IReadOnlyCollection<IExternalGameComponent> _externalGameComponents;
        private readonly ModuleLogger _logger;

        private IGraphicDeviceContext _graphicDeviceContext;
        
        protected GameApp()
        {
            if (_oneInstanceCreated) throw new FrameworkMonoGameException(Strings.Exceptions.Constructing.MultipleGameInstances, GetType().Name);
            _oneInstanceCreated = true;

            _externalGameComponents = AppContext.ServiceLocator.Resolve<IExternalGameComponentsService>().GetComponents();
            _logger = new ModuleLogger(LogCategories.GameCore);
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
            IsDisposed = true;
            
            foreach (var component in _externalGameComponents)
            {
                component.Dispose();
            }

            Dispose();
            
            ScenesController.Dispose();
            _graphicDeviceContext?.Dispose();
            
            DisposedEvent?.Invoke(this, EventArgs.Empty);
            DisposedEvent = null;
        }

        void IGameHost.OnInitialize()
        {
            foreach (var component in _externalGameComponents)
            {
                try
                {
                    component.Initialize();
                }
                catch (Exception e)
                {
                    _logger.Error("External game component '{0}' initialization failed: ", e, component);
                }
            }

            try
            {
                _graphicDeviceContext = GraphicsPipelineFactoryService.CreateGraphicDeviceContext();
                OnInitialized();
            }
            catch (Exception e)
            {
                _logger.Error("Game app class initialization error: ", e);
            }
        }

        void IGameHost.OnLoadContent()
        {
            foreach (var component in _externalGameComponents)
            {
                try
                {
                    component.LoadContent();
                }
                catch (Exception e)
                {
                    _logger.Error("External game component '{0}' content loading failed : ", e, component);
                }
            }

            try
            {
                OnContentLoaded();   
            }
            catch (Exception e)
            {
                _logger.Error("Game app class content loaded handler error: ", e);
            }
        }

        void IUpdatable.Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
            
            try
            {
                Update(gameTime);
            }
            catch (Exception e)
            {
                _logger.Error("Game app class Update unhandled exception: ", e);
            }
            
            
            foreach (var component in _externalGameComponents)
            {
                try
                {
                    component.Update(gameTime);
                }
                catch (Exception e)
                {
                    _logger.Error("External game component '{0}' Update unhandled exception : ", e, component);
                }
            }

            if (ScenesController.CanSceneChange)
                ScenesController.CurrentScene = CurrentScene;
            
            ScenesController.Update(gameTime);
        }

        void IDrawable.Draw(GameTime gameTime)
        {
            ScenesController.CurrentScene.GraphicsPipeline.Process(gameTime, _graphicDeviceContext);

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
