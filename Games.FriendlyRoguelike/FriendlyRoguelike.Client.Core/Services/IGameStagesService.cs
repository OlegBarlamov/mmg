using FriendlyRoguelike.Core.Models;

namespace FriendlyRoguelike.Core.Services
{
    public interface IGameStagesService
    {
        GameStages ActiveStage { get; }
        
        bool StageSwitchingInProgress { get; }

        void SwitchToStage(GameStages stage);

        bool CanSwitchTheStage();
    }
}