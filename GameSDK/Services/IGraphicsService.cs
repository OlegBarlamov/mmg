using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameSDK.Services
{
	public interface IGraphicsService
	{
		GraphicsDeviceManager GraphicsDeviceManager { get; }
		GraphicsDevice GraphicsDevice { get; }
		SpriteBatch SpriteBatch { get; }
	}
}
