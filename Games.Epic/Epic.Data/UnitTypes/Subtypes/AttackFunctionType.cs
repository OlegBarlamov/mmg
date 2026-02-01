using System;
using System.Collections.Generic;

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
        int PierceThrough { get; }
        int Splash { get; }
        
        IReadOnlyList<Guid> ApplyBuffTypeIds { get; }
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
        public int PierceThrough { get; set; } = 0;
        public int Splash { get; set; } = 0;
        
        public List<Guid> ApplyBuffTypeIds { get; set; } = new List<Guid>();
        
        IReadOnlyList<Guid> IAttackFunctionType.ApplyBuffTypeIds => ApplyBuffTypeIds;

        public List<string> ApplyBuffTypes { get; set; } = new List<string>();

        public AttackFunctionType()
        {
            
        }
        
        public void CopyFrom(IAttackFunctionType source)
        {
            Name = source.Name;
            ThumbnailUrl = source.ThumbnailUrl;
            AttackMaxRange = source.AttackMaxRange;
            AttackMinRange = source.AttackMinRange;
            StayOnly = source.StayOnly;
            CounterattacksCount = source.CounterattacksCount;
            CounterattacksPenaltyPercentage = source.CounterattacksPenaltyPercentage;
            BulletsCount = source.BulletsCount;
            CanTargetCounterattack = source.CanTargetCounterattack;
            RangePenalty = source.RangePenalty;
            RangePenaltyZonesCount = source.RangePenaltyZonesCount;
            EnemyInRangeDisablesAttack = source.EnemyInRangeDisablesAttack;
            MinDamage = source.MinDamage;
            MaxDamage = source.MaxDamage;
            AttacksCount = source.AttacksCount;
            MovesBackAfterAttack = source.MovesBackAfterAttack;
            PierceThrough = source.PierceThrough;
            Splash = source.Splash;
            ApplyBuffTypeIds = source.ApplyBuffTypeIds != null 
                ? new List<Guid>(source.ApplyBuffTypeIds) 
                : new List<Guid>();
        }
    }
}