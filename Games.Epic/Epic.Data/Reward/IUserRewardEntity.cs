using System;

namespace Epic.Data.Reward
{
    internal interface IUserRewardEntity
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid RewardId { get; }
        public bool Accepted { get; }
    }
    
    internal class UserRewardEntity : IUserRewardEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RewardId { get; set; }
        public bool Accepted { get; set; }
    }
}