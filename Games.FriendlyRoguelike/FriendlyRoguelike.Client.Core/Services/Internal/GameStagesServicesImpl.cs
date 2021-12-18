using System;
using FriendlyRoguelike.Core.Models;
using JetBrains.Annotations;

namespace FriendlyRoguelike.Core.Services.Internal
{
    [UsedImplicitly]
    internal class GameStagesServicesImpl : IGameStagesService
    {
        public GameStages ActiveStage => RootModel.CurrentStage;
        
        public bool StageSwitchingInProgress { get; }
        
        private GameRootModel RootModel { get; }

        public GameStagesServicesImpl([NotNull] GameRootModel rootModel)
        {
            RootModel = rootModel ?? throw new ArgumentNullException(nameof(rootModel));
        }
        
        public void SwitchToStage(GameStages stage)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSwitchTheStage()
        {
            throw new System.NotImplementedException();
        }
    }
}