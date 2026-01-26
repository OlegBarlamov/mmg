using System;
using Epic.Core.Services.BuffTypes;
using Epic.Data.Buff;

namespace Epic.Core.Services.Buffs
{
    public class MutableBuffObject : IBuffObject, IBuffEntityFields
    {
        public Guid Id { get; set; }
        public Guid BuffTypeId { get; set; }
        public Guid TargetBattleUnitId { get; set; }
        public int DurationRemaining { get; set; }

        public IBuffTypeObject BuffType { get; set; }

        private MutableBuffObject() { }

        public static MutableBuffObject FromEntity(IBuffEntity entity)
        {
            return new MutableBuffObject
            {
                Id = entity.Id,
                BuffTypeId = entity.BuffTypeId,
                TargetBattleUnitId = entity.TargetBattleUnitId,
                DurationRemaining = entity.DurationRemaining,
                BuffType = null,
            };
        }

        public IBuffEntity ToEntity()
        {
            return BuffEntity.FromFields(Id, this);
        }
    }
}

