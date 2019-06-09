﻿using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game
{
    public interface IUpdatable
    {
        void Update(GameTime gameTime);
    }

	public interface IUpdatable<TState>
	{
		TState Update(GameTime gameTime);
	}
}