using System;
using Epic.Core.Objects;
using Epic.Data.Battles;

namespace Epic.Core.Services.Battles
{
    public interface IPlayerInBattleInfoObject : IGameObject<IPlayerToBattleEntity>
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        bool RansomClaimed { get; }
        bool RunClaimed { get; }
        PlayerRunInfo RunInfo { get; }
        int? LastRoundMagicUsed { get; }
    }

    public class MutablePlayerInBattleInfoObject : IPlayerInBattleInfoObject
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public bool RansomClaimed { get; set; }
        
        public bool RunClaimed { get; set; }
        public PlayerRunInfo RunInfo { get; set; }
        public int? LastRoundMagicUsed { get; set; }

        private MutablePlayerInBattleInfoObject(Guid id)
        {
            Id = id;
        }

        public static MutablePlayerInBattleInfoObject FromEntity(IPlayerToBattleEntity entity)
        {
            return new MutablePlayerInBattleInfoObject(entity.Id)
            {
                PlayerId = entity.PlayerId,
                RansomClaimed = entity.RansomClaimed,
                RunClaimed = entity.RunClaimed,
                RunInfo = entity.RunClaimed ? new PlayerRunInfo
                {
                    RunClaimedUnitIndex = entity.RunClaimedUnitIndex.Value,
                    RunRoundsRemaining = entity.RunRoundsRemaining.Value,
                } : null,
                LastRoundMagicUsed = entity.LastRoundMagicUsed
            };
        }

        public IPlayerToBattleEntity ToEntity()
        {
            return new PlayerToBattleEntity
            {
                Id = Id,
                PlayerId = PlayerId,
                RansomClaimed = RansomClaimed,
                RunClaimed = RunClaimed,
                RunClaimedUnitIndex = RunInfo?.RunClaimedUnitIndex,
                RunRoundsRemaining = RunInfo?.RunRoundsRemaining,
                LastRoundMagicUsed = LastRoundMagicUsed,
            };
        }
    }
}