using System;

namespace Epic.Data.Buff
{
    public interface IBuffEntityFields
    {
        Guid BuffTypeId { get; }
        Guid TargetBattleUnitId { get; }
        int DurationRemaining { get; }
    }

    public interface IBuffEntity : IBuffEntityFields
    {
        Guid Id { get; set; }
    }

    public class BuffEntityFields : IBuffEntityFields
    {
        public Guid BuffTypeId { get; set; }
        public Guid TargetBattleUnitId { get; set; }
        public int DurationRemaining { get; set; }
    }

    public class BuffEntity : BuffEntityFields, IBuffEntity
    {
        public Guid Id { get; set; }

        internal void FillFrom(IBuffEntityFields fields)
        {
            BuffTypeId = fields.BuffTypeId;
            TargetBattleUnitId = fields.TargetBattleUnitId;
            DurationRemaining = fields.DurationRemaining;
        }

        public static BuffEntity FromFields(Guid id, IBuffEntityFields fields)
        {
            var entity = new BuffEntity { Id = id };
            entity.FillFrom(fields);
            return entity;
        }
    }
}