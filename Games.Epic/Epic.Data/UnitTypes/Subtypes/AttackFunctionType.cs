namespace Epic.Data.UnitTypes.Subtypes
{
    public interface IAttackFunctionType
    {
        string Name { get; }
        string ThumbnailUrl { get; }
        int AttackMaxRange { get; }
        int AttackMinRange { get; }
        bool StayOnly { get; }
        int CounterattacksCount { get; }
        int CounterattacksPenaltyPercentage { get; }
        int BulletsCount { get; }
        bool CanTargetCounterattack { get; }
        bool RangePenalty { get; }
        /**
         * 1 - splits the range in two and applies 50% penalty for the furthest zone
         */
        int RangePenaltyZonesCount { get; }
        int EnemyInRangeDisablesAttack { get; }
        int MinDamage { get; }
        int MaxDamage { get; }
        int AttacksCount { get; }
        bool MovesBackAfterAttack { get; }
    } 
    
    public class AttackFunctionType : IAttackFunctionType
    {
        public string Name { get; set; } = "Unknown";
        public string ThumbnailUrl { get; set; }
        public int AttackMaxRange { get; set; } = 1;
        public int AttackMinRange { get; set; } = 1;
        public bool StayOnly { get; set; }
        public int CounterattacksCount { get; set; } = 1;
        public int CounterattacksPenaltyPercentage { get; set; }
        public int BulletsCount { get; set; } = int.MaxValue;
        public bool CanTargetCounterattack { get; set; } = true;
        public bool RangePenalty { get; set; } = false;
        public int RangePenaltyZonesCount { get; set; }
        public int EnemyInRangeDisablesAttack { get; set; }
        public int MinDamage { get; set; } = 1;
        public int MaxDamage { get; set; } = 1;
        public int AttacksCount { get; set; } = 1;
        public bool MovesBackAfterAttack { get; set; }

        public AttackFunctionType()
        {
            
        }
    }
}