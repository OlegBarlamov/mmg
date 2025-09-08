using System;

namespace Epic.Data.Battles
{
    public interface IPlayerToBattleEntity
    {
        Guid Id { get; }
        Guid PlayerId { get; }
        Guid BattleId { get; }
        bool ClaimedRansom { get; }
    }

    internal class PlayerToBattleEntity : IPlayerToBattleEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid BattleId { get; set; }
        public bool ClaimedRansom { get; set; }
    }
}