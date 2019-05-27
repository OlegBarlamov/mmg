using Microsoft.Xna.Framework;

namespace FrameworkSDK
{
	public interface IView
	{
		void Render(GameTime gameTime);

		void Update(GameTime gameTime);
	}
}
