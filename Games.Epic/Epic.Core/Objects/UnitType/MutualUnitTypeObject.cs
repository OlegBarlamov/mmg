using System;

namespace Epic.Core
{
    public class MutualUnitTypeObject : IUnitTypeObject
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