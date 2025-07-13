using System;

namespace Epic.Data.UnitTypes
{
    public interface IUnitTypeProperties : IUnitProps
    {
        string BattleImgUrl { get; }
        public string DashboardImgUrl { get; set; }
    }
    
    public interface IUnitTypeEntity : IUnitTypeProperties
    {
        Guid Id { get; }
    }

    internal class UnitTypeEntity : IUnitTypeEntity
    {
        public Guid Id { get; set; }
        public int Speed { get; set; }
        public int AttackMaxRange { get; set; }
        public int AttackMinRange { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string BattleImgUrl { get; set; }
        public string DashboardImgUrl { get; set; }
    }
}