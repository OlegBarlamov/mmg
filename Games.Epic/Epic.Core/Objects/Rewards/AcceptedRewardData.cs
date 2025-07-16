using System;

namespace Epic.Core.Objects.Rewards
{
    public class AcceptedRewardData
    {
        public Guid RewardId { get; set; }
        public Guid UserId { get; set; }
        public IUserUnitObject[] UnitsGiven { get; set; }

        public static AcceptedRewardData Empty(Guid rewardId, Guid userId)
        {
            return new AcceptedRewardData
            {
                RewardId = rewardId,
                UserId = userId,
                UnitsGiven = Array.Empty<IUserUnitObject>(),
            };
        }
    }
}