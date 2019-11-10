using System;
using FrameworkSDK.MonoGame;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Gates.ClientCore
{
	[UsedImplicitly]
    public sealed class GameHost : IGameHost
    {
	    private IExternalCommandsProvider ExternalCommandsProvider { get; }

	    private IGameHeart _gameHeart;

	    public GameHost([NotNull] IExternalCommandsProvider externalCommandsProvider)
	    {
		    ExternalCommandsProvider = externalCommandsProvider ?? throw new ArgumentNullException(nameof(externalCommandsProvider));
			ExternalCommandsProvider.NewCommand += OnNewExternalCommand;
	    }

	    public void Run()
        {
			_gameHeart.Run(this);
		}

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        public void Dispose()
        {
	        ExternalCommandsProvider.NewCommand -= OnNewExternalCommand;
			ExternalCommandsProvider.Dispose();
		}

        public void Initialize(IGameHeart gameHeart)
        {
	        _gameHeart = gameHeart;

	        ExternalCommandsProvider.Open();
		}

	    private void OnNewExternalCommand(string commandLine)
	    {
	    }
	}
}
