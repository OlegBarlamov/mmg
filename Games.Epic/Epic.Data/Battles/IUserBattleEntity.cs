using System;

namespace Epic.Data.Battles
{
    public interface IUserBattleEntity
    {
        Guid Id { get; }
        Guid UserId { get; }
        Guid BattleId { get; }
    }
}