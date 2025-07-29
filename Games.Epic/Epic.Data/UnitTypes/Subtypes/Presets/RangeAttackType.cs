namespace Epic.Data.UnitTypes.Subtypes.Presets
{
    public class RangeAttackType : AttackFunctionType
    {
        public RangeAttackType()
        {
            Name = "Range";
            ThumbnailUrl = null;
            MinDamage = 1;
            MaxDamage = 1;
            AttackMaxRange = 7;
            AttackMinRange = 2;
            StayOnly = true;
            CounterattacksCount = 0;
            CounterattacksPenaltyPercentage = 0;
            RangePenalty = true;
            RangePenaltyZonesCount = 1;
            EnemyInRangeDisablesAttack = 1;
        }
    }
}