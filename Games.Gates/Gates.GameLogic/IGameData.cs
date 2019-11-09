using System.Collections.Generic;
using Gates.Core.Models;

namespace Gates.GameLogic
{
    public interface IGameData
    {
        int GameTimeMs { get; }

        IReadOnlyList<PlayerGameData> PlayersGameData { get; }

        IReadOnlyList<MachineData> SceneMachines { get; }
    }
}
