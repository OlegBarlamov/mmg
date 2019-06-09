using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Scenes
{
	public abstract class Scene : IUpdatable, IClosable, IDrawable, INamed
	{
		public void Update(GameTime gameTime)
		{
			
		}

		public void OnClosed()
		{
			
		}

		public void OnOpened()
		{
			
		}

		ClosingState IUpdatable<ClosingState>.Update(GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime)
		{
			
		}

		public string Name { get; set; }
	}
}
