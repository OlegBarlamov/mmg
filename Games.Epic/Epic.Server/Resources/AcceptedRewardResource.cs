using System;
using System.Linq;
using Epic.Core.Objects.Rewards;

namespace Epic.Server.Resources
{
    public class AcceptedRewardResource
    {
        public Guid RewardId { get; }
        public Guid UserId { get; }
        public UserUnitInDashboardResource[] UsersGiven { get; }
        public AcceptedRewardResource(AcceptedRewardData acceptedRewardData)
        {
            RewardId = acceptedRewardData.RewardId;
            UserId = acceptedRewardData.UserId;
            UsersGiven = acceptedRewardData.UnitsGiven.Select(x => new UserUnitInDashboardResource(x)).ToArray();
        }
    }
}