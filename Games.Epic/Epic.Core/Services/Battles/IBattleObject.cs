using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Data.Battles;

namespace Epic.Core.Services.Battles
{
    public interface IBattleObject : IGameObject<IBattleEntity>
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        int TurnNumber { get; }
        int Width { get; }
        int Height { get; }
        bool IsActive { get; }
        int TurnPlayerIndex { get; }
        int LastTurnUnitIndex { get; }
        
        IReadOnlyList<Guid> PlayersIds { get; }
        
        IReadOnlyCollection<IBattleUnitObject> Units { get; }

        Guid? GetPlayerId(InBattlePlayerNumber playerNumber);
    }
}