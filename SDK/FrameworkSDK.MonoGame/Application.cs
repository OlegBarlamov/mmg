using System;
using FrameworkSDK.MonoGame.GameStructure;
using FrameworkSDK.MonoGame.GameStructure.Mapping;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using AppContext = FrameworkSDK.MonoGame.AppContext;

namespace FrameworkSDK
{
	public abstract class Application : IGameHost 
	{
	    public abstract Scene CurrentScene { get; }

        private IGame Game { get; set; }

	    private IScenesResolver ScenesResolver { get; set; }
        private IScenesContainer ScenesContainer { get; }

        protected Application()
	    {
	        var serviceLocator = AppContext.ServiceLocator;
	        ScenesContainer = serviceLocator.Resolve<IScenesContainer>();
	    }

	    protected virtual void Update(GameTime gameTime)
	    {

	    }

	    protected virtual void RegisterScenes(IScenesRegistrator scenesRegistrator)
	    {
            
	    }

	    [NotNull] protected Scene ResolveScene([NotNull] object model)
	    {
	        if (model == null) throw new ArgumentNullException(nameof(model));

	        return ScenesResolver.ResolveScene(model);
	    }

	    private void Initialize()
	    {
	        RegisterScenes(ScenesContainer);
	        ScenesResolver = ScenesContainer.CreateResolver();
	    }

        void IGameHost.Run([NotNull] IGame game)
		{
		    Game = game ?? throw new ArgumentNullException(nameof(game));

		    try
		    {
		        Initialize();

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
            ScenesContainer.Dispose();
            ScenesResolver.Dispose();
		    Game.Dispose();
		}
	}
}
