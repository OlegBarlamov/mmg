using System;
using FrameworkSDK.Localization;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;
using FrameworkSDK.IoC;
using Microsoft.Xna.Framework.Content;
using IUpdateable = FrameworkSDK.MonoGame.Mvc.IUpdateable;
using IDrawable = FrameworkSDK.MonoGame.Mvc.IDrawable;

namespace FrameworkSDK.MonoGame
{
	public abstract class GameApplication : IGameHost 
	{
	    public abstract Scene CurrentScene { get; }

        private IGameHeart GameHeart { get; set; }

        private IScenesController ScenesController { get; }

	    protected GameApplication()
	    {
	        ScenesController = AppContext.ServiceLocator.Resolve<IScenesController>();
	    }

	    protected virtual void Dispose()
	    {

	    }

	    protected virtual void Update(GameTime gameTime)
	    {

	    }

	    protected virtual void Initialize()
	    {

	    }

	    protected virtual void Draw(GameTime gameTime)
	    {
		    
	    }

	    protected IGameHeart Game => GameHeart;
	    
	    protected ContentManager Content => ((Game) GameHeart).Content;

	    void IGameHost.Initialize(IGameHeart gameHeart)
	    {
	        GameHeart = gameHeart ?? throw new ArgumentNullException(nameof(gameHeart));

	        try
	        {
	            Initialize();
	        }
	        catch (Exception e)
	        {
	            throw new FrameworkMonoGameException(Strings.Exceptions.FatalException, e);
	        }
        }

        void IApplication.Run()
		{
		    try
		    {
                GameHeart.Run(this);
		    }
		    catch (Exception e)
		    {
		        throw new FrameworkMonoGameException(Strings.Exceptions.FatalException, e);
		    }
		    finally
		    {
		        GameHeart.Stop();
            }
        }

		void IUpdateable.Update(GameTime gameTime)
		{
            Update(gameTime);

		    if (ScenesController.CanSceneChange)
		        ScenesController.CurrentScene = CurrentScene;

            ScenesController.Update(gameTime);
		}

	    void IDrawable.Draw(GameTime gameTime)
	    {
            ScenesController.Draw(gameTime);
            
            Draw(gameTime);
	    }

        void IDisposable.Dispose()
		{
		    GameHeart.Dispose();

		    Dispose();
		}
	}
}
