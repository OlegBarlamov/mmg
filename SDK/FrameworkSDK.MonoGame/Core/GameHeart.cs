using System;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.Services;
using FrameworkSDK.MonoGame.Services.Implementations;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Exceptions;

namespace FrameworkSDK.MonoGame
{
    [UsedImplicitly]
	internal sealed class GameHeart : Microsoft.Xna.Framework.Game, IGameHeart
	{
		public event Action ResourceLoading;
		public event Action ResourceUnloading;
		
	    [NotNull] private GraphicsDeviceManager GraphicsDeviceManager { get; }

        [NotNull] private IGameHost GameApp { get; set; }

        [NotNull] private ModuleLogger Logger { get; }

	    [NotNull] private IGameParameters Parameters { get; }

        [NotNull] private IGameHeartServices GameHeartServices { get; }

        [NotNull] private IDebugInfoService DebugInfoService { get; }

        [NotNull] private AppStateService AppStateService { get; }

        public GameHeart(
	        [NotNull] IFrameworkLogger logger,
	        [NotNull] IGameParameters parameters,
	        [NotNull] IGameHeartServices gameHeartServices,
	        [NotNull] AppStateService appStateService,
	        [NotNull] IDebugInfoService debugInfoService)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		    GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
		    DebugInfoService = debugInfoService ?? throw new ArgumentNullException(nameof(debugInfoService));
		    AppStateService = appStateService ?? throw new ArgumentNullException(nameof(appStateService));
		    
		    GraphicsDeviceManager = new GraphicsDeviceManager(this);
		    GraphicsDeviceManager.PreparingDeviceSettings += GraphicsDeviceManagerOnPreparingDeviceSettings;

		    Logger = new ModuleLogger(logger, LogCategories.GameCore);

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

			    GameApp = CreateGameAppInstance();
			    GameApp.DisposedEvent += GameHostOnDisposed;

			    // Loading content
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

		    Logger.Info("Initialize delayed components...");
		    ProcessDelayedOperationOnGameReady();
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
			    DebugInfoService.StartMeasure(nameof(Update));
			    
			    ProcessAppStateDelayedUpdateActions(gameTime);
			    
			    GameApp.Update(gameTime);
			    
			    base.Update(gameTime);
		    }
		    finally
		    {
			    DebugInfoService.StopMeasure(nameof(Update));
			    AppStateService.IsUpdateStateActive = false;
		    }
	    }

	    protected override void Draw(GameTime gameTime)
	    {
		    try
		    {
			    AppStateService.IsDrawStateActive = true;
			    DebugInfoService.StartMeasure(nameof(Draw));
			    
			    ProcessAppStateDelayedDrawActions(gameTime);
			    
			    GraphicsDevice.Clear(Color.CornflowerBlue);
			    GameApp.Draw(gameTime);
			    
			    base.Draw(gameTime);
		    }
		    finally
		    {
			    DebugInfoService.StopMeasure(nameof(Draw));
			    AppStateService.IsDrawStateActive = false;
		    }
	    }

	    private IGameHost CreateGameAppInstance()
	    {
		    try
		    {
			    return AppContext.ServiceLocator.Resolve<GameApp>();
		    }
		    catch (Exception e)
		    {
			    if (e.FindInnerException<SceneNotInitializedException>() != null)
				    throw new SceneNotInitializedException("Attempt adding scene components in a scene constructor. Use Initialize function instead", e);
			    
			    throw new FrameworkMonoGameException("Error while creating game host instance", e);
		    }
	    }

	    private void SetupParameters([NotNull] IGameParameters parameters)
	    {
	        Logger.Info("Setup gameHeart parameters...");

	        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

	        Content.RootDirectory = parameters.ContentRootDirectory;

	        IsMouseVisible = parameters.IsMouseVisible;
	        IsFixedTimeStep = parameters.IsFixedTimeStamp;
	        TargetElapsedTime = parameters.TargetElapsedTime;
	        
	        GraphicsDeviceManager.PreferredBackBufferWidth = parameters.BackBufferSize.Width;
	        GraphicsDeviceManager.PreferredBackBufferHeight = parameters.BackBufferSize.Height;
	        GraphicsDeviceManager.IsFullScreen = parameters.IsFullScreenMode;
	        GraphicsDeviceManager.SynchronizeWithVerticalRetrace = parameters.SynchronizeWithVerticalRetrace;
	        // TODO  Antializcing
	        GraphicsDeviceManager.ApplyChanges();
	    }
	    
	    private void GameHostOnDisposed(object sender, EventArgs eventArgs)
	    {
		    GameApp.DisposedEvent -= GameHostOnDisposed;
		    GraphicsDeviceManager.PreparingDeviceSettings -= GraphicsDeviceManagerOnPreparingDeviceSettings;
		    
		    Activated -= OnAppActivated;
		    Deactivated -= OnAppDeactivated;

		    ResourceLoading = null;
		    ResourceUnloading = null;
		    
		    Dispose(true);

		    GraphicsDeviceManager.Dispose();

		    Logger.Dispose();
	    }
	    
	    private void ProcessDelayedOperationOnGameReady()
	    {
		    while (!AppStateService.DelayedOnAppReadyActions.IsEmpty)
		    {
			    if (AppStateService.DelayedOnAppReadyActions.TryDequeue(out var action))
			    {
				    try
				    {
					    action();
				    }
				    catch (Exception e)
				    {
					    Logger.Error("Error while processing a delayed action on game ready", e);
				    }
			    }
		    }
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
					    Logger.Error("Error while processing a delayed update action", e);
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
					    Logger.Error("Error while processing a delayed draw action", e);
				    }
			    }
		    }
	    }

	    private void GraphicsDeviceManagerOnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	    {
		    GraphicsDeviceManager.PreparingDeviceSettings -= GraphicsDeviceManagerOnPreparingDeviceSettings;
		    e.GraphicsDeviceInformation.GraphicsProfile = Parameters.GraphicsProfile;
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
