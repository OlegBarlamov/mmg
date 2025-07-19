using System;

namespace Epic.Data.Battles
{
    internal interface IPlayerToBattleEntity
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        Guid BattleId { get; }
    }

    internal class PlayerToBattleEntity : IPlayerToBattleEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid BattleId { get; set; }
    }
}