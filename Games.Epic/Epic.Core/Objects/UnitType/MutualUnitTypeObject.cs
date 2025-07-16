using System;
using Epic.Data.UnitTypes;

namespace Epic.Core
{
    public class MutualUnitTypeObject : IUnitTypeObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public int AttackMaxRange { get; set; }
        public int AttackMinRange { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string BattleImgUrl { get; set; }
        public string DashboardImgUrl { get; set; }

        private MutualUnitTypeObject()
        {
            
        }
        
        public static MutualUnitTypeObject FromEntity(IUnitTypeEntity entity)
        {
            return new MutualUnitTypeObject
            {
                Id = entity.Id,
                Name = entity.Name,
                AttackMinRange = entity.AttackMinRange,
                AttackMaxRange = entity.AttackMaxRange,
                BattleImgUrl = entity.BattleImgUrl,
                DashboardImgUrl = entity.DashboardImgUrl,
                Damage = entity.Damage,
                Speed = entity.Speed,
                Health = entity.Health,
            };
        }
    }
}