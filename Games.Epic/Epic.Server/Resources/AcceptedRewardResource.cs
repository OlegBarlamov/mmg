using System;
using System.Linq;
using Epic.Core.Objects.Rewards;
using Epic.Data.GameResources;

namespace Epic.Server.Resources
{
    public class AcceptedRewardResource
    {
        public Guid RewardId { get; }
        public Guid PlayerId { get; }
        public UserUnitInDashboardResource[] UsersGiven { get; }
        public ResourceDashboardResource[] ResourcesGiven { get; } 
        public AcceptedRewardResource(AcceptedRewardData acceptedRewardData)
        {
            RewardId = acceptedRewardData.RewardId;
            PlayerId = acceptedRewardData.PlayerId;
            UsersGiven = acceptedRewardData.UnitsGiven.Select(x => new UserUnitInDashboardResource(x)).ToArray();
            ResourcesGiven = acceptedRewardData.ResourcesGiven.Select(x => new ResourceDashboardResource(x)).ToArray();
        }
    }
}