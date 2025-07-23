namespace Epic.Data.UnitTypes.Subtypes
{
    public interface IAttackFunctionType
    {
        string Name { get; }
        string ThumbnailUrl { get; }
        int AttackMaxRange { get; }
        int AttackMinRange { get; }
        bool StayOnly { get; }
        bool CounterattackAllowed { get; }
        int CounterattackPenaltyPercentage { get; }
        bool RangePenalty { get; }
        /**
         * 1 - splits the range in two and applies 50% penalty for the furthest zone
         */
        int RangePenaltyZonesCount { get; }
        int EnemyInRangeDisablesAttack { get; }
        int MinDamage { get; }
        int MaxDamage { get; }
    } 
    
    public class AttackFunctionType : IAttackFunctionType
    {
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public int AttackMaxRange { get; set; }
        public int AttackMinRange { get; set; }
        public bool StayOnly { get; set; }
        public bool CounterattackAllowed { get; set; }
        public int CounterattackPenaltyPercentage { get; set; }
        public bool RangePenalty { get; set; }
        public int RangePenaltyZonesCount { get; set; }
        public int EnemyInRangeDisablesAttack { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
    }
}