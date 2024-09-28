using System;

namespace Epic.Data.BattleDefinitions
{
    public interface IUserBattleDefinitionEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        Guid UserId { get; }
    }
}