using System;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK
{
	public abstract class Application : IGameHost 
	{
	    public abstract Scene CurrentScene { get; }

        private IGame Game { get; set; }

	    protected virtual void Update(GameTime gameTime)
	    {

	    }

        void IGameHost.Run([NotNull] IGame game)
		{
		    Game = game ?? throw new ArgumentNullException(nameof(game));

		    try
		    {
		        Game.Run();
		    }
		    catch (Exception e)
		    {
		        throw new FrameworkException(Strings.Exceptions.FatalException, e);
		    }
		    finally
		    {
		        Game.Stop();
            }
        }

		void IUpdatable.Update(GameTime gameTime)
		{
			Update(gameTime);
		}

		void IDisposable.Dispose()
		{
		    Game.Dispose();
		}
	}
}
