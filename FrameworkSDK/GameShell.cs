using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.Constructing;
using FrameworkSDK.Game;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK
{
	internal sealed class GameShell : Microsoft.Xna.Framework.Game
	{
		[NotNull]
		public List<ISubsystem> Subsystems { get; } = new List<ISubsystem>();
		[NotNull]
		private IApplication Application { get; }
		[NotNull]
		private GraphicsDeviceManager GraphicsDeviceManager { get; }
		[NotNull]
		private ModuleLogger Logger { get; }

		[NotNull]
		private IScenesController ScenesController { get; set; }
		[NotNull]
		private SpriteBatch SpriteBatch { get; set; }

		public GameShell([NotNull] IApplication application, [NotNull] IFrameworkLogger logger)
		{
		    if (logger == null) throw new ArgumentNullException(nameof(logger));
			Application = application ?? throw new ArgumentNullException(nameof(application));

			GraphicsDeviceManager = new GraphicsDeviceManager(this);

		    Logger = new ModuleLogger(logger, FrameworkLogModule.GameCore);
        }

		public void SetupParameters([NotNull] GameStartParameters startParameters)
		{
		    Logger.Info("Setup start parameters...");

            if (startParameters == null) throw new ArgumentNullException(nameof(startParameters));

		    Content.RootDirectory = startParameters.ContentRootDirectory;

		    GraphicsDeviceManager.PreferredBackBufferWidth = startParameters.BackBufferSize.Width;
		    GraphicsDeviceManager.PreferredBackBufferHeight = startParameters.BackBufferSize.Height;
		    GraphicsDeviceManager.IsFullScreen = startParameters.IsFullScreenMode;
		}

	    public void Stop()
	    {
	        foreach (var subsystem in Subsystems)
	        {
	            subsystem.Stop();
	        }

	        GraphicsDeviceManager.Dispose();

			Logger.Dispose();
        }

		public void RegisterServices(IServiceRegistrator serviceRegistrator)
		{
			
		}

		public void ResolveDependencies(IServiceLocator serviceLocator)
		{
			ScenesController = serviceLocator.Resolve<IScenesController>();

			RandomShell.Setup(serviceLocator.Resolve<IRandomService>());
		}

		protected override void Initialize()
		{
		    Logger.Info("Initialize...");

		    foreach (var subsystem in Subsystems)
		    {
		        subsystem.Start();
		    }

            base.Initialize();
		}

		protected override void LoadContent()
		{
		    Logger.Info("Loading content...");

            base.LoadContent();

			SpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			Logger.Info("Unloading content...");

			SpriteBatch.Dispose();

			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
	    {
			Application.Update(gameTime);

		    if (ScenesController.CanSceneChanged)
			    ScenesController.CurrentScene = Application.CurrentScene;

		    ScenesController.Update(gameTime);

			base.Update(gameTime);
	    }

	    protected override void Draw(GameTime gameTime)
	    {
		    ScenesController.Draw(gameTime);

	        base.Draw(gameTime);
	    }	    
	}
}
