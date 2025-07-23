namespace Epic.Data.UnitTypes.Subtypes.Presets
{
    public class MeleeAttackType : AttackFunctionType
    {
        public MeleeAttackType()
        {
            Name = "Melee";
            ThumbnailUrl = null;
            MinDamage = 1;
            MaxDamage = 1;
            AttackMaxRange = 1;
            AttackMinRange = 1;
            StayOnly = false;
            CounterattackAllowed = true;
            CounterattackPenaltyPercentage = 50;
            RangePenalty = false;
            RangePenaltyZonesCount = 0;
            EnemyInRangeDisablesAttack = 0;
        }
    }
}