using JetBrains.Annotations;

namespace Epic.Core.Logic
{
    public interface IGameModeProvider
    {
        IGameMode GetGameMode();
    }

    [UsedImplicitly]
    public class FixedGameModeProvider : IGameModeProvider
    {
        private IGameMode _gameMode;
        
        public IGameMode GetGameMode()
        {
            return _gameMode;
        }

        public void SetGameMode(IGameMode gameMode)
        {
            _gameMode = gameMode;
        }
    }
}