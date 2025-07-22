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
        public List<AttackFunctionType> Attacks { get; set; } = new List<AttackFunctionType>();
        IReadOnlyList<IAttackFunctionType> IUnitProps.Attacks => Attacks;
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
                BattleImgUrl = entity.BattleImgUrl,
                DashboardImgUrl = entity.DashboardImgUrl,
                Speed = entity.Speed,
                Health = entity.Health,
                Attacks = entity.Attacks.Cast<AttackFunctionType>().ToList(),
            };
        }

        public IUnitTypeEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}