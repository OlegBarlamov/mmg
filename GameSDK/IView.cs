using Microsoft.Xna.Framework;

namespace GameSDK
{
	public interface IView
	{
		void Render(GameTime gameTime);

		void Update(GameTime gameTime);
	}
}
