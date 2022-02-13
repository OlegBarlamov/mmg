using System;
using System.Collections.Concurrent;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame
{
    [UsedImplicitly]
	internal sealed class GameHeart : Microsoft.Xna.Framework.Game, IGameHeart
	{
		public event Action ResourceLoading;
		public event Action ResourceUnloading;
		
	    [NotNull]
        private GraphicsDeviceManager GraphicsDeviceManager { get; }

        [NotNull]
        private IGameHost GameApp { get; }

        [NotNull]
		private ModuleLogger Logger { get; }

	    [NotNull]
        private IGameParameters Parameters { get; }

        [NotNull]
        private IGameHeartServices GameHeartServices { get; }
        
        [NotNull]
        private AppStateService AppStateService { get; }
        
        public GameHeart(
	        [NotNull] GameApp gameApp,
	        [NotNull] IFrameworkLogger logger,
	        [NotNull] IGameParameters parameters,
	        [NotNull] IGameHeartServices gameHeartServices,
	        [NotNull] IAppStateService appStateService)
		{
			if (gameApp == null) throw new ArgumentNullException(nameof(gameApp));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		    GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
		    AppStateService = (AppStateService) appStateService;
		    
		    GraphicsDeviceManager = new GraphicsDeviceManager(this);
		    GameApp = gameApp;

		    GameApp.DisposedEvent += GameHostOnDisposed;
		    
		    Logger = new ModuleLogger(logger, FrameworkLogModule.GameCore);

		    Activated += OnAppActivated;
		    Deactivated += OnAppDeactivated;
		    
		    AppStateService.IsAppFocused = IsActive;
		    SetupParameters(Parameters);
		}

        protected override void Initialize()
		{
		    Logger.Info("Initialize...");
		    AppStateService.IsInitializing = true;

		    try
		    {
			    ((GameHeartServicesHolder)GameHeartServices).Initialize(this, GraphicsDeviceManager, Content, Services);
			    
			    base.Initialize();
			    
			    GameApp.OnInitialize();
			    
			    AppStateService.IsInitialized = true;
		    }
		    finally
		    {
			    AppStateService.IsInitializing = false;
		    }
		}

		protected override void LoadContent()
		{
		    Logger.Info("Loading content...");

		    AppStateService.CoreResourceLoading = true;

		    try
		    {
			    ResourceLoading?.Invoke();
		    
			    base.LoadContent();
            
			    GameApp.OnLoadContent();

			    AppStateService.CoreResourceLoaded = true;
		    }
		    finally
		    {
			    AppStateService.CoreResourceLoading = false;
		    }
		}

		protected override void UnloadContent()
		{
			Logger.Info("Unloading content...");

			AppStateService.CoreResourceUnloading = true;

			try
			{
				ResourceUnloading?.Invoke();

				GameApp.OnUnloadContent();
			}
			finally
			{
				AppStateService.CoreResourceUnloading = false;
			}

			base.UnloadContent();
			
			AppStateService.CoreResourceLoaded = false;
		}

		protected override void Update(GameTime gameTime)
	    {
		    try
		    {
			    AppStateService.IsUpdateStateActive = true;
			    
			    ProcessAppStateDelayedUpdateActions(gameTime);
			    
			    GameApp.Update(gameTime);

			    base.Update(gameTime);
		    }
		    finally
		    {
			    AppStateService.IsUpdateStateActive = false;
		    }
	    }

	    protected override void Draw(GameTime gameTime)
	    {
		    try
		    {
			    AppStateService.IsDrawStateActive = true;
			    
			    ProcessAppStateDelayedDrawActions(gameTime);
			    
			    GraphicsDevice.Clear(Color.CornflowerBlue);
			    GameApp.Draw(gameTime);
			    
			    base.Draw(gameTime);
		    }
		    finally
		    {
			    AppStateService.IsDrawStateActive = false;
		    }
	    }

	    private void SetupParameters([NotNull] IGameParameters parameters)
	    {
	        Logger.Info("Setup gameHeart parameters...");

	        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

	        Content.RootDirectory = parameters.ContentRootDirectory;

	        IsMouseVisible = parameters.IsMouseVisible;
	        GraphicsDeviceManager.PreferredBackBufferWidth = parameters.BackBufferSize.Width;
	        GraphicsDeviceManager.PreferredBackBufferHeight = parameters.BackBufferSize.Height;
	        GraphicsDeviceManager.IsFullScreen = parameters.IsFullScreenMode;
	        GraphicsDeviceManager.ApplyChanges();
	    }
	    
	    private void GameHostOnDisposed(object sender, EventArgs eventArgs)
	    {
		    Activated -= OnAppActivated;
		    Deactivated -= OnAppDeactivated;
		    
		    ResourceLoading?.Invoke();
		    ResourceUnloading?.Invoke();
		    
		    Dispose(true);

		    GraphicsDeviceManager.Dispose();

		    Logger.Dispose();
	    }

	    private void ProcessAppStateDelayedUpdateActions(GameTime gameTime)
	    {
		    while (!AppStateService.DelayedUpdateActions.IsEmpty)
		    {
			    if (AppStateService.DelayedUpdateActions.TryDequeue(out var action))
			    {
				    try
				    {
					    action(gameTime);
				    }
				    catch (Exception e)
				    {
					    //TODO
					    throw;
				    }
			    }
		    }
	    }
	    
	    private void ProcessAppStateDelayedDrawActions(GameTime gameTime)
	    {
		    while (!AppStateService.DelayedDrawActions.IsEmpty)
		    {
			    if (AppStateService.DelayedDrawActions.TryDequeue(out var action))
			    {
				    try
				    {
					    action(gameTime);
				    }
				    catch (Exception e)
				    {
					    //TODO
					    throw;
				    }
			    }
		    }
	    }

	    private void OnAppDeactivated(object sender, EventArgs e)
	    {
		    AppStateService.IsAppFocused = IsActive;
	    }

	    private void OnAppActivated(object sender, EventArgs e)
	    {
		    AppStateService.IsAppFocused = IsActive;
	    }

	    void IGameHeart.Run()
	    {
		    try
		    {
			    AppStateService.IsRunning = true;
			    base.Run(Parameters.GameRunBehavior);
		    }
		    finally
		    {
			    AppStateService.IsRunning = false;
		    }
	    }
	}
}
