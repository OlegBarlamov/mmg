using System;
using FriendlyRoguelike.Core.Models;
using FriendlyRoguelike.Core.Services;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace FriendlyRoguelike.Core.Components
{
    [UsedImplicitly]
    internal class GameMainComponent : IGameComponent
    {
        private IGameStagesService GameStagesService { get; }
        private StartingGameComponent StartingGameComponent { get; }
        private MainMenuGameComponent MainMenuGameComponent { get; }
        private ExitingComponent ExitingComponent { get; }
        private GameInProgressComponent GameInProgressComponent { get; }

        private ILogger Logger { get; }

        public GameMainComponent(
            [NotNull] IGameStagesService gameStagesService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] StartingGameComponent startingGameComponent,
            [NotNull] MainMenuGameComponent mainMenuGameComponent,
            [NotNull] ExitingComponent exitingComponent,
            [NotNull] GameInProgressComponent gameInProgressComponent)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            GameStagesService = gameStagesService ?? throw new ArgumentNullException(nameof(gameStagesService));
            StartingGameComponent = startingGameComponent ?? throw new ArgumentNullException(nameof(startingGameComponent));
            MainMenuGameComponent = mainMenuGameComponent ?? throw new ArgumentNullException(nameof(mainMenuGameComponent));
            ExitingComponent = exitingComponent ?? throw new ArgumentNullException(nameof(exitingComponent));
            GameInProgressComponent = gameInProgressComponent ?? throw new ArgumentNullException(nameof(gameInProgressComponent));
            Logger = loggerFactory.CreateLogger("Game.Main");
        }
        
        public void Update(GameTimeTicks gameTime)
        {
            if (GameStagesService.ActiveStage == GameStages.Exiting)
            {
                ExitingComponent.Update(gameTime);
            }
            
            if (GameStagesService.ActiveStage == GameStages.None)
            {
                if (GameStagesService.CanSwitchTheStage())
                {
                    GameStagesService.SwitchToStage(GameStages.Starting);
                }
                else
                {
                    if (!GameStagesService.StageSwitchingInProgress)
                    {
                        Logger.Error("%%%: Invalid game stage");
                    }
                }
            }

            if (GameStagesService.ActiveStage == GameStages.Starting)
            {
                StartingGameComponent.Update(gameTime);
                if (StartingGameComponent.IsLoaded && GameStagesService.CanSwitchTheStage())
                {
                    GameStagesService.SwitchToStage(GameStages.MainMenu);
                }
            }

            if (GameStagesService.ActiveStage == GameStages.MainMenu)
            {
                MainMenuGameComponent.Update(gameTime);
                if (MainMenuGameComponent.IsExited && GameStagesService.CanSwitchTheStage())
                {
                    GameStagesService.SwitchToStage(GameStages.Exiting);
                }
            }

            if (GameStagesService.ActiveStage == GameStages.GameLoading)
            {
                
            }
            
            if (GameStagesService.ActiveStage == GameStages.Game)
            {
                
            }
        }
    }
}