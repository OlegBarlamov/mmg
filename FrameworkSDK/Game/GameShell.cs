using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.Game
{
    [UsedImplicitly]
	internal sealed class GameShell : Microsoft.Xna.Framework.Game, IGame
	{
	    public SpriteBatch SpriteBatch { get; set; }

        [NotNull]
		private IGameHost Host { get; }
		[NotNull]
		private GraphicsDeviceManager GraphicsDeviceManager { get; }
		[NotNull]
		private ModuleLogger Logger { get; }

		[NotNull]
		private IScenesController ScenesController { get; }

		public GameShell([NotNull] IGameHost host, [NotNull] IFrameworkLogger logger, [NotNull] IScenesController scenesController)
		{
		    if (logger == null) throw new ArgumentNullException(nameof(logger));
		    Host = host ?? throw new ArgumentNullException(nameof(host));

		    ScenesController = scenesController ?? throw new ArgumentNullException(nameof(scenesController));
            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger = new ModuleLogger(logger, FrameworkLogModule.GameCore);
        }

		public void SetupParameters([NotNull] StartParameters startParameters)
		{
		    Logger.Info("Setup start parameters...");

            if (startParameters == null) throw new ArgumentNullException(nameof(startParameters));

		    Content.RootDirectory = startParameters.ContentRootDirectory;

		    GraphicsDeviceManager.PreferredBackBufferWidth = startParameters.BackBufferSize.Width;
		    GraphicsDeviceManager.PreferredBackBufferHeight = startParameters.BackBufferSize.Height;
		    GraphicsDeviceManager.IsFullScreen = startParameters.IsFullScreenMode;
		}

		protected override void Initialize()
		{
		    Logger.Info("Initialize...");

            base.Initialize();
		}

		protected override void LoadContent()
		{
		    Logger.Info("Loading content...");

		    SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
		}

		protected override void UnloadContent()
		{
			Logger.Info("Unloading content...");

			SpriteBatch.Dispose();

			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
	    {
			Host.Update(gameTime);

		    if (ScenesController.CanSceneChange)
			    ScenesController.CurrentScene = Host.CurrentScene;

		    ScenesController.Update(gameTime);

			base.Update(gameTime);
	    }

	    protected override void Draw(GameTime gameTime)
	    {
		    ScenesController.Draw(gameTime);

	        base.Draw(gameTime);
	    }

	    void IDisposable.Dispose()
	    {
            Dispose(true);

	        GraphicsDeviceManager.Dispose();

	        Logger.Dispose();
        }

	    public void Stop()
	    {
	        
	    }
	}
}
