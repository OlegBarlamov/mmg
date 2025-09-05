using System;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;

namespace Epic.Core.Services.GameResources.Errors
{
    public class UnitCantBeUpgradedInTheReward : Exception
    {
        public UnitCantBeUpgradedInTheReward(IGlobalUnitObject unitType, IRewardObject reward) 
            : base($"{unitType.UnitType.Id} can not be upgraded in reward {reward.Id}")
        {
        }
    }
}