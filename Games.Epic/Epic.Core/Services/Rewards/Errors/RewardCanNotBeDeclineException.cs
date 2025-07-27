using System;

namespace Epic.Core.Services.Rewards.Errors
{
    public class RewardCanNotBeDeclineException : Exception
    {
        public RewardCanNotBeDeclineException() : base("Reward can't be decline")
        {
            
        }
    }
}