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
        public UnitInDashboardResource[] UnitsGiven { get; }
        public ResourceDashboardResource[] ResourcesGiven { get; }
        
        public AcceptedRewardResource(AcceptedRewardData acceptedRewardData)
        {
            RewardId = acceptedRewardData.RewardId;
            PlayerId = acceptedRewardData.PlayerId;
            UnitsGiven = acceptedRewardData.UnitsGiven.Select(x => new UnitInDashboardResource(x)).ToArray();
            ResourcesGiven = acceptedRewardData.ResourcesGiven.Select(x => new ResourceDashboardResource(x)).ToArray();
        }
    }
}