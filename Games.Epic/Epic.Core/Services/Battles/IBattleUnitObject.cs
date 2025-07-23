using System;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;

namespace Epic.Core.Services.Battles
{
    public interface IBattleUnitObject : IGameObject<IBattleUnitEntity>, IBattlePositioned
    {
        Guid Id { get; }
        Guid BattleId { get; }
        IGlobalUnitObject GlobalUnit { get; }
        int PlayerIndex { get; }
        int CurrentHealth { get; }
    }
}