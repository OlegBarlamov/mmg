using System;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Core
{
    [UsedImplicitly]
    internal class GameHeartServicesHolder : IGameHeartServices
    {
        public event Action ResourceLoading;
        public event Action ResourceUnloading;
        public event Action Loaded;

        public bool IsServicesLoaded => _initialized;

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                CheckInitialized();
                return _graphicsDeviceManager;
            }
        }

        public ContentManager RootContentManager
        {
            get
            {
                CheckInitialized();
                return _rootContentManager;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                CheckInitialized();
                return _rootSpriteBatch;
            }
        }

        public GameServiceContainer MonoGameServiceContainer
        {
            get
            {
                CheckInitialized();
                return _monoGameServiceContainer;
            }
        }

        private GraphicsDeviceManager _graphicsDeviceManager;
        private ContentManager _rootContentManager;
        private GameServiceContainer _monoGameServiceContainer;
        private SpriteBatch _rootSpriteBatch;
        private IGameHeart _gameHeart;
        private bool _initialized;

        public void Initialize(
            [NotNull] IGameHeart gameHeart,
            [NotNull] GraphicsDeviceManager graphicsDeviceManager,
            [NotNull] ContentManager rootContentManager,
            [NotNull] GameServiceContainer gameServiceContainer)
        {
            _gameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));
            _graphicsDeviceManager = graphicsDeviceManager ?? throw new ArgumentNullException(nameof(graphicsDeviceManager));
            _rootContentManager = rootContentManager ?? throw new ArgumentNullException(nameof(rootContentManager));
            _monoGameServiceContainer = gameServiceContainer ?? throw new ArgumentNullException(nameof(gameServiceContainer));
            
            _rootSpriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice); 
            
            _gameHeart.ResourceLoading += GameHeartOnResourceLoading;
            _gameHeart.ResourceUnloading += GameHeartOnResourceUnloading;
            
            _initialized = true;
            Loaded?.Invoke();
        }

        public void Dispose()
        {
            ResourceLoading = null;
            ResourceUnloading = null;
            Loaded = null;
            
            _gameHeart.ResourceLoading -= GameHeartOnResourceLoading;
            _gameHeart.ResourceUnloading -= GameHeartOnResourceUnloading;
        }

        private void GameHeartOnResourceLoading()
        {
            ResourceLoading?.Invoke();
        }
        
        private void GameHeartOnResourceUnloading()
        {
            ResourceUnloading?.Invoke();
        }

        private void CheckInitialized()
        {
            if (!_initialized) throw new FrameworkMonoGameException(Strings.Exceptions.GameHeartServicesNotInitialized);
        }
    }
}