using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Data.UnitTypes;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Core.Services.UnitTypes
{
    public class MutualUnitTypeObject : IUnitTypeObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MovementType Movement { get; set; }
        public List<AttackFunctionType> Attacks { get; set; } = new List<AttackFunctionType>();
        IReadOnlyList<IAttackFunctionType> IUnitProps.Attacks => Attacks;
        public string BattleImgUrl { get; set; }
        public string DashboardImgUrl { get; set; }
        public int Value { get; set; }
        public IReadOnlyDictionary<string, int> ResourcesDistribution { get; set; }
        public IReadOnlyList<Guid> UpgradeForUnitTypeIds { get; set; }

        private MutualUnitTypeObject()
        {
            
        }
        
        public static MutualUnitTypeObject FromEntity(IUnitTypeEntity entity)
        {
            return new MutualUnitTypeObject
            {
                Id = entity.Id,
                Name = entity.Name,
                BattleImgUrl = entity.BattleImgUrl,
                DashboardImgUrl = entity.DashboardImgUrl,
                Speed = entity.Speed,
                Health = entity.Health,
                Attacks = entity.Attacks.Cast<AttackFunctionType>().ToList(),
                Value = entity.Value,
                ResourcesDistribution = entity.ResourcesDistribution,
                Attack = entity.Attack,
                Defense = entity.Defense,
                Movement = entity.Movement,
                UpgradeForUnitTypeIds = entity.UpgradeForUnitTypeIds
            };
        }

        public IUnitTypeEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}