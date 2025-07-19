using System;

namespace Epic.Data.Reward
{
    internal interface IPlayerRewardEntity
    {
        public Guid Id { get; }
        public Guid PlayerId { get; }
        public Guid RewardId { get; }
        public bool Accepted { get; }
    }
    
    internal class PlayerRewardEntity : IPlayerRewardEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RewardId { get; set; }
        public bool Accepted { get; set; }
    }
}