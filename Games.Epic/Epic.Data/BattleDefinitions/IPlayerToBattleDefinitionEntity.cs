using System;

namespace Epic.Data.BattleDefinitions
{
    internal interface IPlayerToBattleDefinitionEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        Guid PlayerId { get; }
    }

    internal class PlayerToBattleDefinitionEntity : IPlayerToBattleDefinitionEntity
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
