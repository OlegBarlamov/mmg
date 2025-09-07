using System;

namespace Epic.Core.Services.Rewards.Errors
{
    public class RewardGuardBattleIsNotFinishedException: Exception
    {
        public RewardGuardBattleIsNotFinishedException() : base("Reward's battle is not finished.")
        {
            
        }
    }
}