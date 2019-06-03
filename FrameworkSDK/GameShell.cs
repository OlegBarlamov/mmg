using System;
using System.Collections.Generic;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK
{
	internal sealed class GameShell : Microsoft.Xna.Framework.Game
	{
		public List<ISubsystem> Subsystems { get; } = new List<ISubsystem>();

		private GraphicsDeviceManager GraphicsDeviceManager { get; }

		private SpriteBatch SpriteBatch { get; set; }

	    private ModuleLogger Logger { get; }

		public GameShell([NotNull] IFrameworkLogger logger)
		{
		    if (logger == null) throw new ArgumentNullException(nameof(logger));

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
        }

		public void RegisterServices(IServiceRegistrator serviceRegistrator)
		{
			
		}

		public void ResolveDependencies(IServiceLocator serviceLocator)
		{
			
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

	    protected override void Update(GameTime gameTime)
	    {
	        base.Update(gameTime);
	    }

	    protected override void Draw(GameTime gameTime)
	    {
	        base.Draw(gameTime);
	    }

	    protected override void UnloadContent()
	    {
	        Logger.Info("Unloading content...");

            SpriteBatch.Dispose();

            base.UnloadContent();
	    }

	    
	}
}
