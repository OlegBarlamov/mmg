using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.Services
{
	public interface IGraphicsService
	{
		GraphicsDeviceManager GraphicsDeviceManager { get; }
		GraphicsDevice GraphicsDevice { get; }
		SpriteBatch SpriteBatch { get; }
	}
}
