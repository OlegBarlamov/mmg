using System;
using System.Collections.Generic;
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
        int InitialCount { get; }
        int CurrentCount { get; }
        bool Waited { get; }
        int CurrentAttack { get; set; }
        int CurrentDefense { get; set; }
        IReadOnlyList<AttackFunctionStateEntity> AttackFunctionsData { get; }
    }
}