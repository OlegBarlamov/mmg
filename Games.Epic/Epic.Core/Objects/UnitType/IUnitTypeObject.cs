using System;

namespace Epic.Core
{
    public interface IUnitTypeObject
    {
        Guid Id { get; }
        
        int Speed { get; }
        int AttackMaxRange { get; }
        int AttackMinRange { get; }
        int Damage { get; }
        int Health { get; }
        
        string BattleImgUrl { get; }
        string DashboardImgUrl { get; }
    }
}