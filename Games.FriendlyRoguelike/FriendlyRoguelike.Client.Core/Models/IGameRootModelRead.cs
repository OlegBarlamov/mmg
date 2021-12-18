using FriendlyRoguelike.Core.Models.Stages;

namespace FriendlyRoguelike.Core.Models
{
    public interface IGameRootModelRead
    {
        GameStages CurrentStage { get; }
        
        StageModelStarting StageModelStarting { get; }
        
        StageModelMainMenu StageModelMainMenu { get; }
        
        StageModelPause StageModelPause { get; }
        
        StageModelGame StageModelGame { get; }
        
        StageModelExiting StageModelExiting { get; }
    }
}