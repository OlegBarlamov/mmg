using System;

namespace Epic.Data.UnitTypes
{
    public interface IUnitTypeEntity
    {
        Guid Id { get; }
        int Speed { get; }
        int AttackMaxRange { get; }
        int AttackMinRange { get; }
        int Damage { get; }
        int Health { get; }
        
        string BattleIconUrl { get; } 
    }
}