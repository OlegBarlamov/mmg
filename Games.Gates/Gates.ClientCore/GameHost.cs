using System;
using FrameworkSDK.MonoGame;
using Gates.ClientCore.ExternalCommands;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Gates.ClientCore
{
	[UsedImplicitly]
    internal sealed class GameHost : IGameHost
    {
	    private IExternalCommandsProvider ExternalCommandsProvider { get; }
        private IExternalCommandsProcessor ExternalCommandsProcessor { get; }

        private IGameHeart _gameHeart;

	    public GameHost(
	        [NotNull] IExternalCommandsProvider externalCommandsProvider,
	        [NotNull] IExternalCommandsProcessor externalCommandsProcessor)
	    {
		    ExternalCommandsProvider = externalCommandsProvider ?? throw new ArgumentNullException(nameof(externalCommandsProvider));
	        ExternalCommandsProcessor = externalCommandsProcessor ?? throw new ArgumentNullException(nameof(externalCommandsProcessor));
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
	        ExternalCommandsProcessor.ProcessCommand(commandLine);
        }
    }
}
