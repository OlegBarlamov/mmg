using FriendlyRoguelike.Core.Models.Stages;

namespace FriendlyRoguelike.Core.Models
{
    internal class GameRootModel : IGameRootModelRead
    {
        public GameStages CurrentStage { get; set; } = GameStages.None;
        
        public StageModelStarting StageModelStarting { get; } = new StageModelStarting();
        
        public StageModelMainMenu StageModelMainMenu { get; } = new StageModelMainMenu();
        
        public StageModelPause StageModelPause { get; } = new StageModelPause();
        
        public StageModelGame StageModelGame { get; } = new StageModelGame();
        
        public StageModelExiting StageModelExiting { get; } = new StageModelExiting();
    }
}