using System;
using System.Collections.Generic;
using System.Text;

namespace Gates.Core.Models
{
    public class GameDataIteration
    {
        public int GameTimeSeconds { get; }

        public PlayerGameData[] PlayersGameData { get; }

        public MachineData[] SceneMachines { get; }
    }
}
