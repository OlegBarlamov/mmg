using System;

namespace Epic.Data.Battles
{
    public interface IPlayerToBattleEntity
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        Guid BattleId { get; }
        bool RansomClaimed { get; }
        bool RunClaimed { get; }
        int? RunRoundsRemaining { get; }
        int? RunClaimedUnitIndex { get; }
    }

    public class PlayerToBattleEntity : IPlayerToBattleEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid BattleId { get; set; }
        public bool RansomClaimed { get; set; } = false;
        public bool RunClaimed { get; set; } = false;
        public int? RunRoundsRemaining { get; set; } = null;
        public int? RunClaimedUnitIndex { get; set; }  = null;

        public void UpdateFrom(IPlayerToBattleEntity entity)
        {
            PlayerId = entity.PlayerId;
            BattleId = entity.BattleId;
            RansomClaimed = entity.RansomClaimed;
            RunClaimed = entity.RunClaimed;
            RunRoundsRemaining = entity.RunRoundsRemaining;
            RunClaimedUnitIndex = entity.RunClaimedUnitIndex;
        }
    }
}