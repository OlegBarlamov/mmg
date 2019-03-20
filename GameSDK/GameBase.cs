using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameSDK
{
	public abstract class GameBase : Game
	{
		protected internal GraphicsDeviceManager GraphicsDeviceManager { get; }

		protected internal SpriteBatch SpriteBatch { get; private set; }

		protected GameBase()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
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
