using System.Collections.Generic;
using FrameworkSDK.IoC;
using FrameworkSDK.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK
{
	internal sealed class GameShell : Game
	{
		public List<ISubsystem> Subsystems { get; } = new List<ISubsystem>();

		private GraphicsDeviceManager GraphicsDeviceManager { get; }

		private SpriteBatch SpriteBatch { get; set; }

		public GameShell(IFrameworkLogger logger)
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
		}

		public void SetupParameters(GameParameters parameters)
		{
			Content.RootDirectory = parameters.ContentRootDirectory;
		}

		public void RegisterServices(IServiceRegistrator serviceRegistrator)
		{
			
		}

		public void ResolveDependencies(IServiceLocator serviceLocator)
		{
			
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			SpriteBatch = new SpriteBatch(GraphicsDevice);
		}
	}
}
