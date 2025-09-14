using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.BattleObstacles;
using Epic.Data.Battles;

namespace Epic.Core.Services.Battles
{
    public interface IBattleObject : IGameObject<IBattleEntity>, ISize
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        int TurnNumber { get; }
        bool IsActive { get; }
        int TurnPlayerIndex { get; }
        int LastTurnUnitIndex { get; }
        bool ProgressDays { get; }
        int RoundNumber { get; }
        
        IReadOnlyList<IPlayerInBattleInfoObject> PlayerInfos { get; }
        
        IReadOnlyCollection<IBattleUnitObject> Units { get; }
        
        IReadOnlyCollection<IBattleObstacleObject> Obstacles { get; }
    }
}