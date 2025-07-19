using System;
using System.Linq;
using Epic.Core.Objects.Rewards;

namespace Epic.Server.Resources
{
    public class AcceptedRewardResource
    {
        public Guid RewardId { get; }
        public Guid PlayerId { get; }
        public UserUnitInDashboardResource[] UsersGiven { get; }
        public AcceptedRewardResource(AcceptedRewardData acceptedRewardData)
        {
            RewardId = acceptedRewardData.RewardId;
            PlayerId = acceptedRewardData.PlayerId;
            UsersGiven = acceptedRewardData.UnitsGiven.Select(x => new UserUnitInDashboardResource(x)).ToArray();
        }
    }
}