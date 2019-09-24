using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.GameStructure
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
	    [NotNull]
        private IGameParameters Parameters { get; }

	    public GameShell([NotNull] IGameHost host, [NotNull] IFrameworkLogger logger, [NotNull] IScenesController scenesController,
	        [NotNull] IGameParameters parameters)
		{
		    if (logger == null) throw new ArgumentNullException(nameof(logger));
		    Host = host ?? throw new ArgumentNullException(nameof(host));

		    ScenesController = scenesController ?? throw new ArgumentNullException(nameof(scenesController));
		    Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		    GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger = new ModuleLogger(logger, FrameworkLogModule.GameCore);

            SetupParameters(parameters);
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

	    private void SetupParameters([NotNull] IGameParameters parameters)
	    {
	        Logger.Info("Setup game parameters...");

	        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

	        Content.RootDirectory = parameters.ContentRootDirectory;

	        GraphicsDeviceManager.PreferredBackBufferWidth = parameters.BackBufferSize.Width;
	        GraphicsDeviceManager.PreferredBackBufferHeight = parameters.BackBufferSize.Height;
	        GraphicsDeviceManager.IsFullScreen = parameters.IsFullScreenMode;
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
