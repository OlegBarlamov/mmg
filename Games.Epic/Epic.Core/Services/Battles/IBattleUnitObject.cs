using System;
using System.Collections.Generic;
using Epic.Core.Services.Buffs;
using Epic.Core.Objects;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.Heroes;

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
        
        IHeroStats HeroStats { get; }
        
        int MaxHealth { get; }
        
        int CurrentAttack { get; }
        
        int CurrentDefense { get; }
        
        IReadOnlyList<AttackFunctionStateEntity> AttackFunctionsData { get; }

        IReadOnlyList<IBuffObject> Buffs { get; }
        
        /// <summary>
        /// Calculates the effective MinDamage for an attack, applying buff bonuses.
        /// </summary>
        int GetEffectiveMinDamage(int baseMinDamage);
        
        /// <summary>
        /// Calculates the effective MaxDamage for an attack, applying buff bonuses.
        /// MaxDamage is guaranteed to be at least equal to effective MinDamage.
        /// </summary>
        int GetEffectiveMaxDamage(int baseMinDamage, int baseMaxDamage);
    }
}