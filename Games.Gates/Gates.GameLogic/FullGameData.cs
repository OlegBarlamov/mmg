using System.Collections.Generic;
using Gates.Core.Models;

namespace Gates.GameLogic
{
    internal class FullGameData : IGameData
    {
        public int GameTimeMs { get; set; }

        public List<PlayerGameData> PlayersGameData { get; private set; } = new List<PlayerGameData>();
        public List<MachineData> SceneMachines { get; private set; } = new List<MachineData>();

        IReadOnlyList<PlayerGameData> IGameData.PlayersGameData => PlayersGameData;

        IReadOnlyList<MachineData> IGameData.SceneMachines => SceneMachines;

        public static FullGameData Default()
        {
            return new FullGameData
            {
                PlayersGameData = new List<PlayerGameData>
                {
                    DefaultPlayerData(),
                    DefaultPlayerData()
                },
                SceneMachines = new List<MachineData>()
            };
        }

        private static PlayerGameData DefaultPlayerData()
        {
            return new PlayerGameData
            {
                GoldCount = 10,
                Lifes = 5,
                ParticlesQueue = new List<Particle>(),
                ConstructingParticles = new List<ProgressContainer<Particle>>(),
                Slots = new MachineData[10]
            };
        }
    }
}
