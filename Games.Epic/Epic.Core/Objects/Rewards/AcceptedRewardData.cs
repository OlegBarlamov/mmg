using System;
using Epic.Core.Services.Units;

namespace Epic.Core.Objects.Rewards
{
    public class AcceptedRewardData
    {
        public Guid RewardId { get; set; }
        public Guid PlayerId { get; set; }
        public IPlayerUnitObject[] UnitsGiven { get; set; }

        public static AcceptedRewardData Empty(Guid rewardId, Guid playerId)
        {
            return new AcceptedRewardData
            {
                RewardId = rewardId,
                PlayerId = playerId,
                UnitsGiven = Array.Empty<IPlayerUnitObject>(),
            };
        }
    }
}