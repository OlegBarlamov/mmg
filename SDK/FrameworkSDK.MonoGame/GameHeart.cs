using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Config;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame
{
    [UsedImplicitly]
	internal sealed class GameHeart : Microsoft.Xna.Framework.Game, IGameHeart
	{
	    [NotNull]
        public GraphicsDeviceManager GraphicsDeviceManager { get; }

        public SpriteBatch SpriteBatch { get; set; }

        [NotNull]
        private IGameHost GameHost { get; set; }

        [NotNull]
		private ModuleLogger Logger { get; }

	    [NotNull]
        private IGameParameters Parameters { get; }

	    // ReSharper disable once NotNullMemberIsNotInitialized
	    public GameHeart([NotNull] IFrameworkLogger logger, [NotNull] IGameParameters parameters)
		{
		    if (logger == null) throw new ArgumentNullException(nameof(logger));

		    Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		    GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger = new ModuleLogger(logger, FrameworkLogModule.GameCore);

            SetupParameters(parameters);
        }

	    public void Run([NotNull] IGameHost gameHost)
	    {
	        GameHost = gameHost ?? throw new ArgumentNullException(nameof(gameHost));

            Run(Parameters.GameRunBehavior);
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
	        GameHost.Update(gameTime);

			base.Update(gameTime);
	    }

	    protected override void Draw(GameTime gameTime)
	    {
	        GameHost.Draw(gameTime);

            base.Draw(gameTime);
	    }

	    private void SetupParameters([NotNull] IGameParameters parameters)
	    {
	        Logger.Info("Setup gameHeart parameters...");

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
