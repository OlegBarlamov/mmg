using System;
using System.Collections.Generic;
using Epic.Core.Objects;
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
        
        IReadOnlyList<Guid> PlayersIds { get; }
        IReadOnlyList<Guid> ClaimedRansomPlayerIds { get; }
        
        IReadOnlyCollection<IBattleUnitObject> Units { get; }

        Guid? GetPlayerId(InBattlePlayerNumber playerNumber);
    }
}