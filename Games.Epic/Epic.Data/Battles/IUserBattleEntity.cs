using System;

namespace Epic.Data.Battles
{
    internal interface IUserBattleEntity
    {
        Guid Id { get; }
        Guid UserId { get; }
        Guid BattleId { get; }
    }

    internal class UserBattleEntity : IUserBattleEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BattleId { get; set; }
    }
}