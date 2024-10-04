using System;

namespace Epic.Data.Battles
{
    internal interface IBattleDefinitionToBattleEntity
    {
        Guid Id { get; }
        Guid BattleId { get; }
        Guid BattleDefinitionId { get; }
    }

    internal class BattleDefinitionToBattleEntity : IBattleDefinitionToBattleEntity
    {
        public Guid Id { get; set; }
        public Guid BattleId { get; set; }
        public Guid BattleDefinitionId { get; set; }
    }
}