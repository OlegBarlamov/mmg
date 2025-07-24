using System;
using Epic.Core.Services.Units;
using Epic.Data.GameResources;

namespace Epic.Core.Objects.Rewards
{
    public class AcceptedRewardData
    {
        public Guid RewardId { get; set; }
        public Guid PlayerId { get; set; }
        public IGlobalUnitObject[] UnitsGiven { get; set; }
        public ResourceAmount[] ResourcesGiven { get; set; }

        public static AcceptedRewardData Empty(Guid rewardId, Guid playerId)
        {
            return new AcceptedRewardData
            {
                RewardId = rewardId,
                PlayerId = playerId,
                UnitsGiven = Array.Empty<IGlobalUnitObject>(),
            };
        }
    }
}