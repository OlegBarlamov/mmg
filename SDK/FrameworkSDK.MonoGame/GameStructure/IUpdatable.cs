using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.GameStructure
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
