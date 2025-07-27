using System;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Units;
using Epic.Data.GameResources;
using JetBrains.Annotations;

namespace Epic.Core.Objects.Rewards
{
    public class AcceptedRewardData
    {
        public Guid RewardId { get; set; }
        public Guid PlayerId { get; set; }
        public IGlobalUnitObject[] UnitsGiven { get; set; }
        public ResourceAmount[] ResourcesGiven { get; set; }
        public Price PricePayed { get; set; }
        [CanBeNull] public IBattleObject NextBattle { get; set; }

        public static AcceptedRewardData Empty(Guid rewardId, Guid playerId)
        {
            return new AcceptedRewardData
            {
                RewardId = rewardId,
                PlayerId = playerId,
                UnitsGiven = Array.Empty<IGlobalUnitObject>(),
                ResourcesGiven = Array.Empty<ResourceAmount>(),
                PricePayed = Price.Empty(),
                NextBattle = null,
            };
        }
    }
}