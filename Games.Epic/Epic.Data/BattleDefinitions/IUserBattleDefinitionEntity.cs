using System;

namespace Epic.Data.BattleDefinitions
{
    public interface IUserBattleDefinitionEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        Guid UserId { get; }
    }

    internal class UserBattleDefinitionEntity : IUserBattleDefinitionEntity
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public Guid UserId { get; set; }
    }
}