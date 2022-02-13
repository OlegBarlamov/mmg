using System;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.Mvc;
using Gates.ClientCore.ExternalCommands;
using Gates.ClientCore.Rooms;
using Gates.ClientCore.Scenes;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Gates.ClientCore
{
	[UsedImplicitly]
    internal sealed class GameHost : GameApp
	{
	    protected override Scene CurrentScene => _activeScene;

        [NotNull] private IExternalCommandsProvider ExternalCommandsProvider { get; }
        [NotNull] private IExternalCommandsProcessor ExternalCommandsProcessor { get; }
        [NotNull] private IRoomController RoomController { get; }

	    private Scene _activeScene;
	    private Scene _mainMenuScene;
	    private Scene _gameScene;

        public GameHost(
	        [NotNull] IExternalCommandsProvider externalCommandsProvider,
	        [NotNull] IExternalCommandsProcessor externalCommandsProcessor,
	        [NotNull] IRoomController roomController)
	    {
		    ExternalCommandsProvider = externalCommandsProvider ?? throw new ArgumentNullException(nameof(externalCommandsProvider));
	        ExternalCommandsProcessor = externalCommandsProcessor ?? throw new ArgumentNullException(nameof(externalCommandsProcessor));
	        RoomController = roomController ?? throw new ArgumentNullException(nameof(roomController));
	        ExternalCommandsProvider.NewCommand += OnNewExternalCommand;
	    }

        protected override void Dispose()
        {
	        ExternalCommandsProvider.NewCommand -= OnNewExternalCommand;
			ExternalCommandsProvider.Dispose();
		}

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _mainMenuScene = new MainMenuScene();
            _gameScene = new GameScene();

            ExternalCommandsProvider.Open();
            _activeScene = _mainMenuScene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (RoomController.IsGameConnected)
            {
                _activeScene = _gameScene;
            }
            else
            {
                _activeScene = _mainMenuScene;
            }
        }

        private void OnNewExternalCommand(string commandLine)
	    {
	        ExternalCommandsProcessor.ProcessCommand(commandLine);
        }
    }
}
