using System;
using Epic.Data.UnitTypes.Subtypes;

namespace Epic.Data.BattleUnits
{
    public interface IAttackFunctionStateFields
    {
        public int BulletsCount { get; set; }
        public int CounterattacksUsed { get; set; }
    }

    public class AttackFunctionStateEntity : IAttackFunctionStateFields
    {
        public Guid BattleUnitId { get; }
        public int AttackIndex { get; }
        
        public int BulletsCount { get; set; }
        public int CounterattacksUsed { get; set; }

        private AttackFunctionStateEntity(Guid battleUnitId, int attackIndex)
        {
            BattleUnitId = battleUnitId;
            AttackIndex = attackIndex;
        }

        public void UpdateFrom(IAttackFunctionStateFields fields)
        {
            BulletsCount = fields.BulletsCount;
            CounterattacksUsed = fields.CounterattacksUsed;
        }

        public static AttackFunctionStateEntity FromAttackFunction(Guid battleUnitId, int attackIndex, IAttackFunctionType attackFunction)
        {
            return new AttackFunctionStateEntity(battleUnitId, attackIndex)
            {
                BulletsCount = attackFunction.BulletsCount,
                CounterattacksUsed = 0,
            };
        }
    }
}