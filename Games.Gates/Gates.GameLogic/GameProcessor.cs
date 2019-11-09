using System.Drawing;
using Gates.Core;
using Gates.Core.Models;

namespace Gates.GameLogic
{
    public class GameProcessor : IPlayerHandler
    {
        public IGameData GameData => _gameData;

        private readonly FullGameData _gameData;
        private readonly SceneProcessor _sceneProcessor;

        public GameProcessor()
        {
            _gameData = FullGameData.Default();

            _sceneProcessor = new SceneProcessor(_gameData.SceneMachines);
        }

        public void Process(int elapsedTimeMs)
        {
            _gameData.GameTimeMs += elapsedTimeMs;

            _sceneProcessor.Process(elapsedTimeMs);
        }

        private void AddMachineToScene(MachineData machine)
        {
            _gameData.SceneMachines.Add(machine);
        }

        private void PrepareMachineToSceneForPlayer(MachineData machine, int playerID)
        {
            bool direction = playerID > 0;
            var position = direction ? GameConstants.SceneSize : 0;
            machine.Direction = direction;
            machine.Position = new Point(position, 0);
        }

        bool IPlayerHandler.ProduceMachineFromSlot(int playerID, int slotID)
        {
            var playerData = _gameData.PlayersGameData[playerID];
            var slotData = playerData.Slots[slotID];
            if (slotData == null)
                return false;

            playerData.Slots[slotID] = null;

            PrepareMachineToSceneForPlayer(slotData, playerID);
            AddMachineToScene(slotData);
            return true;
        }

        bool IPlayerHandler.ClearSlot(int playerID, int slotID)
        {
            throw new System.NotImplementedException();
        }

        bool IPlayerHandler.StartConstruct(int playerID, int particleID)
        {
            throw new System.NotImplementedException();
        }

        bool IPlayerHandler.ProduceParticleToSlot(int playerID, int slotID, Point position)
        {
            throw new System.NotImplementedException();
        }

        bool IPlayerHandler.QueueScroll(int playerID)
        {
            throw new System.NotImplementedException();
        }
    }
}
