using System;
using System.Collections.Generic;
using Epic.Core.Objects.BattleUnit;

namespace Epic.Core.Objects.Battle
{
    public interface IBattleObject
    {
        Guid Id { get; }
        int TurnIndex { get; }
        int Width { get; }
        int Height { get; }
        bool IsActive { get; }
        int TurnPlayerIndex { get; }
        
        IReadOnlyCollection<IBattleUnitObject> Units { get; }
    }
}