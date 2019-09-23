using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game
{
    public interface IUpdatable
    {
        void Update(GameTime gameTime);
    }

	public interface IUpdatable<out TState>
	{
		TState UpdateState(GameTime gameTime);
	}
}
